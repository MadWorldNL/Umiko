using System.Diagnostics;
using MadWorldNL.Umiko.Functional;

namespace MadWorldNL.Umiko.ServiceBus;

public class LoggingQueryHandler<TQuery, TResponse>(
    IQueryHandler<TQuery, TResponse> innerHandler,
    ILogger<IQueryHandler<TQuery, TResponse>> logger
    ) : IQueryHandler<TQuery, TResponse> where TQuery : IQuery<TResponse>
{
    private static readonly ActivitySource _queryActivitySource = new("MadWorldNL.Umiko");
    
    public async Task<Result<TResponse>> Handle(TQuery query, CancellationToken cancellationToken)
    {
        var queryName = typeof(TQuery).Name;

        using (_ = _queryActivitySource.StartActivity(queryName))
        {
            logger.LogInformation("Processing command {Command}", queryName);

            var result = await innerHandler.Handle(query, cancellationToken);

            if (result.IsSuccess)
            {
                logger.LogInformation("Completed command {Query}", queryName);
            }
            else
            {
                logger.LogError(result.Error, "Completed query {Query} with error", queryName);
            }
        
            return result;  
        }
    }
}