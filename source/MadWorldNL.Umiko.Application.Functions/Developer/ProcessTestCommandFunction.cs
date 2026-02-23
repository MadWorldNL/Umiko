using MadWorldNL.Umiko.Functional;
using MadWorldNL.Umiko.ServiceBus;
using Microsoft.Extensions.Logging;

namespace MadWorldNL.Umiko.Developer;

public class ProcessTestCommandFunction(IMessageBus messageBus, ILogger<ProcessTestCommandFunction> logger) : ICommandHandler<ProcessTestCommand>
{
    public async Task<Result<bool>> Handle(ProcessTestCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("New command '{Command}' received: {Message}", nameof(ProcessTestCommand), command.Message);

        await messageBus.Publish([ new TestProcessedEvent(command.Message) ]);
        
        return Result<bool>.Success(true);
    }
}