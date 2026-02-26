using MadWorldNL.Umiko.DDD;

namespace MadWorldNL.Umiko.Repositories;

public interface IEventRepository<TAggregate, TId>
    where TAggregate : AggregateRoot<TId>
    where TId : notnull
{
    Task SaveAsync(TAggregate aggregate, CancellationToken cancellationToken = default);
    Task<TAggregate?> LoadAsync(TId id, CancellationToken cancellationToken = default);
}