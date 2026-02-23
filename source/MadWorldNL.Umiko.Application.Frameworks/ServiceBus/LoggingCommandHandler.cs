using System.Diagnostics;
using MadWorldNL.Umiko.Functional;
using MadWorldNL.Umiko.Statistics;

namespace MadWorldNL.Umiko.ServiceBus;

public class LoggingCommandHandler<TCommand>(
    ICommandHandler<TCommand> innerHandler,
    ILogger<ICommandHandler<TCommand>> logger
    ) : ICommandHandler<TCommand> where TCommand : ICommand
{
    private static readonly ActivitySource _commandActivitySource = new("MadWorldNL.Umiko");

    public async Task<Result<bool>> Handle(TCommand command, CancellationToken cancellationToken)
    {
        var commandName = typeof(TCommand).Name;

        using (_ = _commandActivitySource.StartActivity(commandName))
        {
            logger.LogInformation("Processing command {Command}", commandName);

            var result = await innerHandler.Handle(command, cancellationToken);

            MetricsOverview.CommandCounter.Add(1);
            
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
}

public class LoggingCommandHandler<TCommand, TResponse>(
    ICommandHandler<TCommand, TResponse> innerHandler,
    ILogger<ICommandHandler<TCommand, TResponse>> logger
    ) : ICommandHandler<TCommand, TResponse> where TCommand : ICommand<TResponse>
{
    private static readonly ActivitySource _commandActivitySource = new("MadWorldNL.Umiko");

    public async Task<Result<TResponse>> Handle(TCommand command, CancellationToken cancellationToken)
    {
        var commandName = typeof(TCommand).Name;

        using (_ = _commandActivitySource.StartActivity(commandName))
        {
            logger.LogInformation("Processing command {Command}", commandName);

            var result = await innerHandler.Handle(command, cancellationToken);

            MetricsOverview.CommandCounter.Add(1);
            
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
}