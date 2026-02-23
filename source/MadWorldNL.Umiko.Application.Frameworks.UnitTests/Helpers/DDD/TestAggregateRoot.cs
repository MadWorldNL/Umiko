namespace MadWorldNL.Umiko.Tests.Helpers.DDD;

public class TestAggregateRoot : AggregateRoot<int>
{
    public TestAggregateRoot(int id)
    {
        Id = id;
    }

    public void RaiseEvent(IDomainEvent domainEvent) => AddDomainEvent(domainEvent);
}