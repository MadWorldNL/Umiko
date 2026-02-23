using System.Text.Json;
using MadWorldNL.Umiko.ServiceBus;
using RabbitMQ.Client;

namespace MadWorldNL.Umiko;

public sealed class RabbitMqMessageBus : IMessageBus
{
    private readonly IConnection _connection;

    public RabbitMqMessageBus(IConnection connection)
    {
        _connection = connection;
    }

    public async Task Send<TCommand>(TCommand command) where TCommand : ICommand
    {
        var exchangeName = typeof(TCommand).Name;
        var body = JsonSerializer.SerializeToUtf8Bytes(command);

        await using var channel = await _connection.CreateChannelAsync();
        await channel.ExchangeDeclareAsync(exchangeName, ExchangeType.Direct, durable: true);

        var properties = new BasicProperties
        {
            Persistent = true,
            ContentType = "application/json"
        };

        await channel.BasicPublishAsync(exchangeName, routingKey: string.Empty, mandatory: false, basicProperties: properties, body: body);
    }

    public async Task Publish<TEvent>(TEvent[] events) where TEvent : IEvent
    {
        if (events.Length == 0) return;

        var exchangeName = typeof(TEvent).Name;

        await using var channel = await _connection.CreateChannelAsync();
        await channel.ExchangeDeclareAsync(exchangeName, ExchangeType.Fanout, durable: true);

        foreach (var @event in events)
        {
            var body = JsonSerializer.SerializeToUtf8Bytes(@event);
            var properties = new BasicProperties
            {
                Persistent = true,
                ContentType = "application/json"
            };

            await channel.BasicPublishAsync(exchangeName, routingKey: string.Empty, mandatory: false, basicProperties: properties, body: body);
        }
    }
}