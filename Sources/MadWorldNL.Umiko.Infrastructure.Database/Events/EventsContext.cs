using System.Text.Json;
using LanguageExt;
using MadWorldNL.Umiko.DomainDrivenDevelopment;
using Marten;

namespace MadWorldNL.Umiko.Events;

public class EventsContext(IDocumentSession session) : IEventsContext
{
    public async Task<Option<TRootAggregate>> GetById<TRootAggregate>(Guid id) where TRootAggregate : RootAggregate
    {
        var rootAggregate = (TRootAggregate)Activator.CreateInstance(typeof(TRootAggregate), true)!;

        try
        {
            var resolvedEvents = await session.Events.FetchStreamAsync(id);

            foreach (var resolvedEvent in resolvedEvents)
            {
                if (resolvedEvent.Data is IDomainEvent domainEvent)
                {
                    rootAggregate.Apply(domainEvent);
                }
            }

            return rootAggregate;
        }
        catch (Exception exception)
        {
            _ = exception;
            return Option<TRootAggregate>.None;
        }
    }

    public async Task Store<TRootAggregate>(TRootAggregate aggregate) where TRootAggregate : RootAggregate
    {
        var state = await session.Events.FetchStreamStateAsync(aggregate.Id.Value);

        if (state is null)
        {
            session.Events.StartStream<TRootAggregate>(aggregate.Id.Value,
                aggregate.DomainEvents.ToArray<object>());
        }
        else
        {
            session.Events.Append(aggregate.Id.Value, aggregate.DomainEvents.ToArray<object>());
        }

        await session.SaveChangesAsync();

        aggregate.ClearDomainEvents();
    }
}