using MadWorldNL.Umiko.Helpers.DDD;

namespace MadWorldNL.Umiko.StepDefinitions.DDD;

[Binding]
[Scope(Feature = "Aggregate Root")]
public class AggregateRootSteps
{
    private TestAggregateRoot? _aggregateRoot;

    [Given("I have a new aggregate root with ID {int}")]
    public void GivenIHaveANewAggregateRootWithId(int id)
    {
        _aggregateRoot = new TestAggregateRoot(id);
    }

    [When("I add a domain event")]
    public void WhenIAddADomainEvent()
    {
        _aggregateRoot.ShouldNotBeNull();
        _aggregateRoot!.RaiseEvent(new TestDomainEvent(DateTime.UtcNow));
    }

    [When("I add {int} domain events")]
    public void WhenIAddDomainEvents(int count)
    {
        _aggregateRoot.ShouldNotBeNull();
        for (var i = 0; i < count; i++)
        {
            _aggregateRoot!.RaiseEvent(new TestDomainEvent(DateTime.UtcNow));
        }
    }

    [When("I clear the domain events")]
    public void WhenIClearTheDomainEvents()
    {
        _aggregateRoot.ShouldNotBeNull();
        _aggregateRoot!.ClearDomainEvents();
    }

    [Then("it should have no domain events")]
    public void ThenItShouldHaveNoDomainEvents()
    {
        _aggregateRoot.ShouldNotBeNull();
        _aggregateRoot!.GetDomainEvents().ShouldBeEmpty();
    }

    [Then("the domain event count should be {int}")]
    public void ThenTheDomainEventCountShouldBe(int count)
    {
        _aggregateRoot.ShouldNotBeNull();
        _aggregateRoot!.GetDomainEvents().Count.ShouldBe(count);
    }

    [When("I reconstitute from {int} domain event")]
    public void WhenIReconstituteFromDomainEvent(int count)
    {
        _aggregateRoot.ShouldNotBeNull();
        var events = Enumerable.Range(0, count)
            .Select(_ => new TestDomainEvent(DateTime.UtcNow))
            .ToList<IDomainEvent>();
        _aggregateRoot!.Reconstitute(events);
    }

    [Then("the last applied date should be set")]
    public void ThenTheLastAppliedDateShouldBeSet()
    {
        _aggregateRoot.ShouldNotBeNull();
        _aggregateRoot!.LastAppliedOn.ShouldNotBeNull();
    }
}