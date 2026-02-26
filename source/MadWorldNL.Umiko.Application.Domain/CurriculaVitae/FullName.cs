using MadWorldNL.Umiko.DDD;

namespace MadWorldNL.Umiko.CurriculaVitae;

public sealed class FullName : ValueObject
{
    public string FirstName { get; }
    public string LastName { get; }
    
    public FullName(string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(firstName)) throw new ArgumentException("First name is required.", nameof(firstName));
        if (string.IsNullOrWhiteSpace(lastName)) throw new ArgumentException("Last name is required.", nameof(lastName));
        
        FirstName = firstName;
        LastName = lastName;
    }

    
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return FirstName;
        yield return LastName;
    }
}