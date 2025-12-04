using MadWorldNL.Umiko.DomainDrivenDevelopment.DefaultExceptions;

namespace MadWorldNL.Umiko.DomainDrivenDevelopment.DefaultEntities;

public class UniqueId : ValueObject
{
    public Guid Value { get; private init; }
    
    private UniqueId() {} // for ORM

    public UniqueId(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new EmptyException<UniqueId>();
        }
        
        Value = value;
    }

    public static UniqueId Empty => new()
    {
        Value = Guid.Empty
    };

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}