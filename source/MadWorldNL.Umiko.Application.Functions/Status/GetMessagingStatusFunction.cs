using MadWorldNL.Umiko.Functional;
using MadWorldNL.Umiko.ServiceBus;

namespace MadWorldNL.Umiko.Status;

public sealed class GetMessagingStatusFunction(IMessageBusStatusRepository messageBusStatusRepository)
    : IQueryHandler<GetMessagingStatusQuery, GetMessagingStatusResult>
{
    public async Task<Result<GetMessagingStatusResult>> Handle(GetMessagingStatusQuery query, CancellationToken cancellationToken)
    {
        var isConnected = await messageBusStatusRepository.CanConnect(cancellationToken);
        return Result<GetMessagingStatusResult>.Success(new GetMessagingStatusResult(isConnected));
    }
}