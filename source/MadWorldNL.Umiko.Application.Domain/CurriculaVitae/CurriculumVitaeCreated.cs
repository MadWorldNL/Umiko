using MadWorldNL.Umiko.DDD;

namespace MadWorldNL.Umiko.CurriculaVitae;

public sealed record CurriculumVitaeCreated : IDomainEvent
{
    public required Guid Id { get; init; }
    public required FullName FullName { get; init; }
    
    public DateTime OccurredOn { get; init; }
}