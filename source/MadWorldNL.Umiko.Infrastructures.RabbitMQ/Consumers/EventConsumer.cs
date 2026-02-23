using System.Diagnostics;
using System.Text;
using System.Text.Json;
using MadWorldNL.Umiko.ServiceBus;
using MadWorldNL.Umiko.Statistics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace MadWorldNL.Umiko.Consumers;

public sealed class EventConsumer<TEvent> : BackgroundService
    where TEvent : IEvent
{
    private readonly IConnection _connection;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<EventConsumer<TEvent>> _logger;

    private static readonly string ExchangeName = typeof(TEvent).Name;
    private static readonly string QueueName = typeof(TEvent).Name;

    public EventConsumer(IConnection connection, IServiceScopeFactory scopeFactory, ILogger<EventConsumer<TEvent>> logger)
    {
        _connection = connection;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await using var channel = await _connection.CreateChannelAsync(cancellationToken: stoppingToken);

        await channel.ExchangeDeclareAsync(ExchangeName, ExchangeType.Fanout, durable: true, cancellationToken: stoppingToken);
        await channel.QueueDeclareAsync(QueueName, durable: true, exclusive: false, autoDelete: false, cancellationToken: stoppingToken);
        await channel.QueueBindAsync(QueueName, ExchangeName, routingKey: string.Empty, cancellationToken: stoppingToken);
        await channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false, cancellationToken: stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (_, ea) =>
        {
            var parentContext = ExtractActivityContext(ea.BasicProperties.Headers);
            using var activity = TracesOverview.ActivitySource.StartActivity(
                $"process {ExchangeName}",
                ActivityKind.Consumer,
                parentContext);

            try
            {
                var @event = JsonSerializer.Deserialize<TEvent>(ea.Body.Span);
                if (@event is null)
                {
                    await channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: false);
                    return;
                }

                await using var scope = _scopeFactory.CreateAsyncScope();
                var handler = scope.ServiceProvider.GetRequiredService<IEventHandler<TEvent>>();
                var result = await handler.Handle(@event, stoppingToken);

                if (result.IsSuccess)
                {
                    await channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
                }
                else
                {
                    _logger.LogError(result.Error, "Failed to handle event {Event}", typeof(TEvent).Name);
                    await channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: false);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing event {Event}", typeof(TEvent).Name);
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

    private static ActivityContext ExtractActivityContext(IDictionary<string, object?>? headers)
    {
        if (headers is null) return default;

        if (!headers.TryGetValue("traceparent", out var traceparentObj)) return default;

        var traceparent = traceparentObj is byte[] bytes
            ? Encoding.UTF8.GetString(bytes)
            : traceparentObj?.ToString();

        string? tracestate = null;
        if (headers.TryGetValue("tracestate", out var tracestateObj))
            tracestate = tracestateObj is byte[] tsBytes
                ? Encoding.UTF8.GetString(tsBytes)
                : tracestateObj?.ToString();

        return ActivityContext.TryParse(traceparent, tracestate, isRemote: true, out var context)
            ? context
            : default;
    }
}