using MadWorldNL.Umiko.DomainDrivenDevelopment.DefaultEntities;
using MadWorldNL.Umiko.Events;

namespace MadWorldNL.Umiko.DomainDrivenDevelopment;

public abstract class RootAggregate
{
    public string AggregateId => $"{AggregateType}-{Id.Value}";
    public abstract string AggregateType { get; }

    public UniqueId Id { get; protected set; } = UniqueId.Empty;
    
    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();
    
    protected void Raise(IDomainEvent @event)
    {
        _domainEvents.Add(@event);
    }

    public void ClearDomainEvents() => _domainEvents.Clear();
    
    public abstract void Apply(IDomainEvent @event);
}