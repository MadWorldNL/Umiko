using MadWorldNL.Umiko.Functional;
using MadWorldNL.Umiko.ServiceBus;

namespace MadWorldNL.Umiko.Status;

public class GetDatabaseStatusFunction(IStatusRepository statusRepository)
    : IQueryHandler<GetDatabaseStatusQuery, GetDatabaseStatusResult>
{
    public async Task<Result<GetDatabaseStatusResult>> Handle(GetDatabaseStatusQuery query, CancellationToken cancellationToken)
    {
        var isConnected = await statusRepository.CanConnect(cancellationToken);
        return Result<GetDatabaseStatusResult>.Success(new GetDatabaseStatusResult(isConnected));
    }
}