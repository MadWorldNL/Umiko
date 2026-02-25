using System.Diagnostics;
using MadWorldNL.Umiko.Functional;
using MadWorldNL.Umiko.Statistics;

namespace MadWorldNL.Umiko.ServiceBus;

public sealed class LoggingEventHandler<TEvent>(
    IEventHandler<TEvent> innerHandler,
    ILogger<IEventHandler<TEvent>> logger
    ) : IEventHandler<TEvent> where TEvent : IEvent
{
    public async Task<Result<bool>> Handle(TEvent @event, CancellationToken cancellationToken)
    {
        var eventName = typeof(TEvent).Name;

        using (_ = TracesOverview.ActivitySource.StartActivity(eventName))
        {
            logger.LogInformation("Processing event {Event}", eventName);

            var result = await innerHandler.Handle(@event, cancellationToken);

            MetricsOverview.EventCounter.Add(1);

            if (result.IsSuccess)
            {
                logger.LogInformation("Completed event {Event}", eventName);
            }
            else
            {
                logger.LogError(result.Error, "Completed event {Event} with error", eventName);
            }

            return result;
        }
    }
}