using MadWorldNL.Umiko.DDD;
using Marten;

namespace MadWorldNL.Umiko.Repositories;

public sealed class EventRepository<TAggregate, TId>(IDocumentSession session)
    : IEventRepository<TAggregate, TId>
    where TAggregate : AggregateRoot<TId>
    where TId : notnull
{
    public async Task SaveAsync(TAggregate aggregate, CancellationToken cancellationToken = default)
    {
        var events = aggregate.GetDomainEvents();
        if (events.Count == 0) return;

        var streamId = ToStreamId(aggregate.Id);
        session.Events.Append(streamId, events.Cast<object>().ToArray());
        await session.SaveChangesAsync(cancellationToken);

        aggregate.ClearDomainEvents();
    }

    public async Task<TAggregate?> LoadAsync(TId id, CancellationToken cancellationToken = default)
    {
        var streamId = ToStreamId(id);
        var events = await session.Events.FetchStreamAsync(streamId, token: cancellationToken);

        if (events.Count == 0) return null;

        var aggregate = (TAggregate)Activator.CreateInstance(typeof(TAggregate), nonPublic: true)!;
        aggregate.Reconstitute(events.Select(e => e.Data).OfType<IDomainEvent>());

        return aggregate;
    }

    private static Guid ToStreamId(TId id) => id switch
    {
        Guid g => g,
        _ => throw new InvalidOperationException($"Unsupported stream ID type: {typeof(TId).Name}")
    };
}