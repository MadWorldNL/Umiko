using System.Diagnostics;
using System.Text.Json;
using MadWorldNL.Umiko.ServiceBus;
using MadWorldNL.Umiko.Statistics;
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

        using var activity = TracesOverview.ActivitySource.StartActivity($"send {exchangeName}", ActivityKind.Producer);

        await using var channel = await _connection.CreateChannelAsync();
        await channel.ExchangeDeclareAsync(exchangeName, ExchangeType.Direct, durable: true);

        var properties = new BasicProperties
        {
            Persistent = true,
            ContentType = "application/json",
            Headers = BuildTraceHeaders(activity)
        };

        await channel.BasicPublishAsync(exchangeName, routingKey: string.Empty, mandatory: false, basicProperties: properties, body: body);
    }

    public async Task Publish<TEvent>(TEvent[] events) where TEvent : IEvent
    {
        if (events.Length == 0) return;

        var exchangeName = typeof(TEvent).Name;

        using var activity = TracesOverview.ActivitySource.StartActivity($"publish {exchangeName}", ActivityKind.Producer);

        await using var channel = await _connection.CreateChannelAsync();
        await channel.ExchangeDeclareAsync(exchangeName, ExchangeType.Fanout, durable: true);

        var headers = BuildTraceHeaders(activity);

        foreach (var @event in events)
        {
            var body = JsonSerializer.SerializeToUtf8Bytes(@event);
            var properties = new BasicProperties
            {
                Persistent = true,
                ContentType = "application/json",
                Headers = headers
            };

            await channel.BasicPublishAsync(exchangeName, routingKey: string.Empty, mandatory: false, basicProperties: properties, body: body);
        }
    }

    private static Dictionary<string, object?> BuildTraceHeaders(Activity? activity)
    {
        var headers = new Dictionary<string, object?>();

        if (activity is null) return headers;

        var traceFlags = activity.ActivityTraceFlags.HasFlag(ActivityTraceFlags.Recorded) ? "01" : "00";
        headers["traceparent"] = $"00-{activity.TraceId}-{activity.SpanId}-{traceFlags}";

        if (!string.IsNullOrEmpty(activity.TraceStateString))
            headers["tracestate"] = activity.TraceStateString;

        return headers;
    }
}