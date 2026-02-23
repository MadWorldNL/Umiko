using System.Text.Json;
using MadWorldNL.Umiko.ServiceBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace MadWorldNL.Umiko;

public sealed class CommandConsumer<TCommand> : BackgroundService
    where TCommand : ICommand
{
    private readonly IConnection _connection;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<CommandConsumer<TCommand>> _logger;

    private static readonly string ExchangeName = typeof(TCommand).Name;
    private static readonly string QueueName = typeof(TCommand).Name;

    public CommandConsumer(IConnection connection, IServiceScopeFactory scopeFactory, ILogger<CommandConsumer<TCommand>> logger)
    {
        _connection = connection;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await using var channel = await _connection.CreateChannelAsync(cancellationToken: stoppingToken);

        await channel.ExchangeDeclareAsync(ExchangeName, ExchangeType.Direct, durable: true, cancellationToken: stoppingToken);
        await channel.QueueDeclareAsync(QueueName, durable: true, exclusive: false, autoDelete: false, cancellationToken: stoppingToken);
        await channel.QueueBindAsync(QueueName, ExchangeName, routingKey: string.Empty, cancellationToken: stoppingToken);
        await channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false, cancellationToken: stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (_, ea) =>
        {
            try
            {
                var command = JsonSerializer.Deserialize<TCommand>(ea.Body.Span);
                if (command is null)
                {
                    await channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: false);
                    return;
                }

                await using var scope = _scopeFactory.CreateAsyncScope();
                var handler = scope.ServiceProvider.GetRequiredService<ICommandHandler<TCommand>>();
                var result = await handler.Handle(command, stoppingToken);

                if (result.IsSuccess)
                {
                    await channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
                }
                else
                {
                    _logger.LogError(result.Error, "Failed to handle command {Command}", typeof(TCommand).Name);
                    await channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: false);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing command {Command}", typeof(TCommand).Name);
                await channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: false);
            }
        };

        await channel.BasicConsumeAsync(QueueName, autoAck: false, consumer: consumer, cancellationToken: stoppingToken);

        try
        {
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException)
        {
            // Normal shutdown
        }
    }
}