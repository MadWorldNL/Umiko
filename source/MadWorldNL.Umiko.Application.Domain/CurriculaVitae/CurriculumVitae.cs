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
        Apply(new CurriculumVitaeCreated
        {
            Id = Guid.NewGuid(),
            FullName = fullName,
            OccurredOn = DateTime.UtcNow
        });
    }

    public static CurriculumVitae Create(FullName fullName)
    {
        return new CurriculumVitae(fullName);
    }

    private void Apply(CurriculumVitaeCreated @event)
    {
        Id = @event.Id;
        FullName = @event.FullName;
    }
}