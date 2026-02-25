namespace MadWorldNL.Umiko.Helpers.DDD;

public class TestAggregateRoot : AggregateRoot<int>
{
    public TestAggregateRoot(int id)
    {
        Id = id;
    }

    public void RaiseEvent(IDomainEvent domainEvent) => Apply(domainEvent);

    public DateTime? LastAppliedOn { get; private set; }

    private void Apply(TestDomainEvent @event)
    {
        LastAppliedOn = @event.OccurredOn;
    }
}