using System.Reflection;

namespace MadWorldNL.Umiko.DDD;

public abstract class AggregateRoot<TId> : Entity<TId>
    where TId : notnull
{
    private readonly List<IDomainEvent> _domainEvents = [];

    public IReadOnlyList<IDomainEvent> GetDomainEvents() => _domainEvents.AsReadOnly();

    public void ClearDomainEvents() => _domainEvents.Clear();

    public void Reconstitute(IEnumerable<IDomainEvent> history)
    {
        foreach (var @event in history)
            ApplyEvent(@event);
    }

    protected void Apply(IDomainEvent domainEvent)
    {
        ApplyEvent(domainEvent);
        _domainEvents.Add(domainEvent);
    }

    private void ApplyEvent(IDomainEvent domainEvent)
    {
        var method = GetType().GetMethod(
            "When",
            BindingFlags.NonPublic | BindingFlags.Instance,
            [domainEvent.GetType()]);

        method?.Invoke(this, [domainEvent]);
    }
}