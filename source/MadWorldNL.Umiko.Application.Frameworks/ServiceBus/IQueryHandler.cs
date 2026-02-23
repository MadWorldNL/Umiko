using MadWorldNL.Umiko.Functional;

namespace MadWorldNL.Umiko.ServiceBus;

public interface IQueryHandler<in TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
    Task<Result<TResponse>> Handle(TQuery query, CancellationToken cancellationToken);
}
