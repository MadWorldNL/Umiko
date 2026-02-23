using MadWorldNL.Umiko.Functional;
using MadWorldNL.Umiko.ServiceBus;

namespace MadWorldNL.Umiko.Status;

public class GetDatabaseStatusFunction(IStatusRepository statusRepository)
    : IQueryHandler<GetDatabaseStatusQuery, bool>
{
    public async Task<Result<bool>> Handle(GetDatabaseStatusQuery query, CancellationToken cancellationToken)
    {
        var isConnected = await statusRepository.CanConnect(cancellationToken);
        return Result<bool>.Success(isConnected);
    }
}