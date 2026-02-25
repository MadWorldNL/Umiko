using MadWorldNL.Umiko.Helpers.DDD;

namespace MadWorldNL.Umiko.StepDefinitions.DDD;

[Binding]
[Scope(Feature = "Entity")]
public class EntitySteps
{
    private TestEntity? _entity;
    private TestEntity? _otherEntity;

    [Given("I have an entity with ID {int}")]
    public void GivenIHaveAnEntityWithId(int id)
    {
        _entity = new TestEntity(id);
    }

    [Given("I have another entity with ID {int}")]
    public void GivenIHaveAnotherEntityWithId(int id)
    {
        _otherEntity = new TestEntity(id);
    }

    [Then("the entities should be equal")]
    public void ThenTheEntitiesShouldBeEqual()
    {
        _entity.ShouldNotBeNull();
        _otherEntity.ShouldNotBeNull();
        _entity!.Equals(_otherEntity).ShouldBeTrue();
    }

    [Then("the entities should not be equal")]
    public void ThenTheEntitiesShouldNotBeEqual()
    {
        _entity.ShouldNotBeNull();
        _otherEntity.ShouldNotBeNull();
        _entity!.Equals(_otherEntity).ShouldBeFalse();
    }

    [Then("the entity should equal itself")]
    public void ThenTheEntityShouldEqualItself()
    {
        _entity.ShouldNotBeNull();
        _entity!.Equals(_entity).ShouldBeTrue();
    }

    [Then("the entity should not equal null")]
    public void ThenTheEntityShouldNotEqualNull()
    {
        _entity.ShouldNotBeNull();
        _entity!.Equals(null).ShouldBeFalse();
    }

    [Then("the entities should have the same hash code")]
    public void ThenTheEntitiesShouldHaveTheSameHashCode()
    {
        _entity.ShouldNotBeNull();
        _otherEntity.ShouldNotBeNull();
        _entity!.GetHashCode().ShouldBe(_otherEntity!.GetHashCode());
    }

    [Then("the entity == operator should return true")]
    public void ThenTheEntityEqualityOperatorShouldReturnTrue()
    {
        _entity.ShouldNotBeNull();
        _otherEntity.ShouldNotBeNull();
        (_entity == _otherEntity).ShouldBeTrue();
    }

    [Then("the entity != operator should return true")]
    public void ThenTheEntityInequalityOperatorShouldReturnTrue()
    {
        _entity.ShouldNotBeNull();
        _otherEntity.ShouldNotBeNull();
        (_entity != _otherEntity).ShouldBeTrue();
    }
}