using MadWorldNL.Umiko.Functional;
using MadWorldNL.Umiko.ServiceBus;

namespace MadWorldNL.Umiko.Status;

public sealed class GetDatabaseStatusFunction(IDatabaseStatusRepository databaseStatusRepository)
    : IQueryHandler<GetDatabaseStatusQuery, GetDatabaseStatusResult>
{
    public async Task<Result<GetDatabaseStatusResult>> Handle(GetDatabaseStatusQuery query, CancellationToken cancellationToken)
    {
        var isConnected = await databaseStatusRepository.CanConnect(cancellationToken);
        return Result<GetDatabaseStatusResult>.Success(new GetDatabaseStatusResult(isConnected));
    }
}