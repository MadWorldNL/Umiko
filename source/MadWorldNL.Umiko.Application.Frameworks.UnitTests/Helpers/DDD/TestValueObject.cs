namespace MadWorldNL.Umiko.Tests.Helpers.DDD;

public class TestValueObject(string value) : ValueObject
{
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return value;
    }
}

public class OtherTestValueObject(string value) : ValueObject
{
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return value;
    }
}