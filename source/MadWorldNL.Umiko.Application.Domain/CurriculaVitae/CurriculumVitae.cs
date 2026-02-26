using JetBrains.Annotations;
using MadWorldNL.Umiko.DDD;

namespace MadWorldNL.Umiko.CurriculaVitae;

public sealed class CurriculumVitae : AggregateRoot<Guid>
{
    public FullName FullName { get; private set; } = null!;
    
    private CurriculumVitae()
    {
    }

    private CurriculumVitae(Guid id, FullName fullName)
    {
        Apply(new CurriculumVitaeCreated
        {
            Id = id,
            FullName = fullName,
            OccurredOn = DateTime.UtcNow
        });
    }

    public static CurriculumVitae Create(Guid id, FullName fullName)
    {
        return new CurriculumVitae(id, fullName);
    }

    [UsedImplicitly]
    private void When(CurriculumVitaeCreated @event)
    {
        Id = @event.Id;
        FullName = @event.FullName;
    }
}