using MadWorldNL.Umiko.DDD;

namespace MadWorldNL.Umiko.CurriculaVitae;

public sealed class CurriculumVitae : AggregateRoot<Guid>
{
    public FullName FullName { get; private set; } = null!;
    
    private CurriculumVitae()
    {
    }

    private CurriculumVitae(FullName fullName)
    {
        Id = Guid.NewGuid();
        FullName = fullName;
        
        AddDomainEvent(new CurriculumVitaeCreated()
        {
            Id = Id,
            FullName = fullName,
            OccurredOn = DateTime.UtcNow
        });
    }

    public static CurriculumVitae Create(FullName fullName)
    {
        return new CurriculumVitae(fullName);
    }
}