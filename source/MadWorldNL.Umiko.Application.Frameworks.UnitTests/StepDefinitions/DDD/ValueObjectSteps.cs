using MadWorldNL.Umiko.Helpers.DDD;

namespace MadWorldNL.Umiko.StepDefinitions.DDD;

[Binding]
[Scope(Feature = "Value Object")]
public class ValueObjectSteps
{
    private ValueObject? _valueObject;
    private ValueObject? _otherValueObject;

    [Given("I have a value object with value {string}")]
    public void GivenIHaveAValueObjectWithValue(string value)
    {
        _valueObject = new TestValueObject(value);
    }

    [Given("I have another value object with value {string}")]
    public void GivenIHaveAnotherValueObjectWithValue(string value)
    {
        _otherValueObject = new TestValueObject(value);
    }

    [Given("I have a different type value object with value {string}")]
    public void GivenIHaveADifferentTypeValueObjectWithValue(string value)
    {
        _otherValueObject = new OtherTestValueObject(value);
    }

    [Then("the value objects should be equal")]
    public void ThenTheValueObjectsShouldBeEqual()
    {
        _valueObject.ShouldNotBeNull();
        _otherValueObject.ShouldNotBeNull();
        _valueObject!.Equals(_otherValueObject).ShouldBeTrue();
    }

    [Then("the value objects should not be equal")]
    public void ThenTheValueObjectsShouldNotBeEqual()
    {
        _valueObject.ShouldNotBeNull();
        _otherValueObject.ShouldNotBeNull();
        _valueObject!.Equals(_otherValueObject).ShouldBeFalse();
    }

    [Then("the value object should equal itself")]
    public void ThenTheValueObjectShouldEqualItself()
    {
        _valueObject.ShouldNotBeNull();
        _valueObject!.Equals(_valueObject).ShouldBeTrue();
    }

    [Then("the value object should not equal null")]
    public void ThenTheValueObjectShouldNotEqualNull()
    {
        _valueObject.ShouldNotBeNull();
        _valueObject!.Equals(null).ShouldBeFalse();
    }

    [Then("the value objects should have the same hash code")]
    public void ThenTheValueObjectsShouldHaveTheSameHashCode()
    {
        _valueObject.ShouldNotBeNull();
        _otherValueObject.ShouldNotBeNull();
        _valueObject!.GetHashCode().ShouldBe(_otherValueObject!.GetHashCode());
    }

    [Then("the value object == operator should return true")]
    public void ThenTheValueObjectEqualityOperatorShouldReturnTrue()
    {
        _valueObject.ShouldNotBeNull();
        _otherValueObject.ShouldNotBeNull();
        (_valueObject == _otherValueObject).ShouldBeTrue();
    }

    [Then("the value object != operator should return true")]
    public void ThenTheValueObjectInequalityOperatorShouldReturnTrue()
    {
        _valueObject.ShouldNotBeNull();
        _otherValueObject.ShouldNotBeNull();
        (_valueObject != _otherValueObject).ShouldBeTrue();
    }
}