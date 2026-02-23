using MadWorldNL.Umiko.Functional;
using MadWorldNL.Umiko.ServiceBus;
using Microsoft.Extensions.Logging;

namespace MadWorldNL.Umiko.Developer;

public class ProcessTestCommandFunction(ILogger<ProcessTestCommandFunction> logger) : ICommandHandler<ProcessTestCommand>
{
    public Task<Result<bool>> Handle(ProcessTestCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("New command '{Command}' received: {Message}", nameof(ProcessTestCommand), command.Message);
        
        return Task.FromResult(Result<bool>.Success(true));
    }
}