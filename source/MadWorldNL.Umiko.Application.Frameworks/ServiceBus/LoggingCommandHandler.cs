using MadWorldNL.Umiko.Functional;

namespace MadWorldNL.Umiko.ServiceBus;

public class LoggingCommandHandler<TCommand>(
    ICommandHandler<TCommand> innerHandler,
    ILogger<ICommandHandler<TCommand>> logger
    ) : ICommandHandler<TCommand> where TCommand : ICommand
{
    public async Task<Result<bool>> Handle(TCommand command, CancellationToken cancellationToken)
    {
        var commandName = typeof(TCommand).Name;

        logger.LogInformation("Processing command {Command}", commandName);

        var result = await innerHandler.Handle(command, cancellationToken);

        if (result.IsSuccess)
        {
            logger.LogInformation("Completed command {Command}", commandName);
        }
        else
        {
            logger.LogError(result.Error, "Completed command {Command} with error", commandName);
        }

        return result;
    }
}

public class LoggingCommandHandler<TCommand, TResponse>(
    ICommandHandler<TCommand, TResponse> innerHandler,
    ILogger<ICommandHandler<TCommand, TResponse>> logger
    ) : ICommandHandler<TCommand, TResponse> where TCommand : ICommand<TResponse>
{
    public async Task<Result<TResponse>> Handle(TCommand command, CancellationToken cancellationToken)
    {
        var commandName = typeof(TCommand).Name;

        logger.LogInformation("Processing command {Command}", commandName);

        var result = await innerHandler.Handle(command, cancellationToken);

        if (result.IsSuccess)
        {
            logger.LogInformation("Completed command {Command}", commandName);
        }
        else
        {
            logger.LogError(result.Error, "Completed command {Command} with error", commandName);
        }

        return result;
    }
}