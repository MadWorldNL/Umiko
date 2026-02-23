using MadWorldNL.Umiko.Functional;
using MadWorldNL.Umiko.ServiceBus;
using Microsoft.Extensions.Logging;

namespace MadWorldNL.Umiko.Developer;

public sealed class TestProcessedEventFunction(ILogger<TestProcessedEventFunction> logger) : IEventHandler<TestProcessedEvent>
{
    public Task<Result<bool>> Handle(TestProcessedEvent @event, CancellationToken cancellationToken)
    {
        logger.LogInformation("New event '{Event}' received: {Message}", nameof(TestProcessedEvent), @event.Message);
        
        return Task.FromResult(Result<bool>.Success(true));
    }
}