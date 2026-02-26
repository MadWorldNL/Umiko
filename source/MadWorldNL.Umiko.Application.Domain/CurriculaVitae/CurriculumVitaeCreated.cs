using MadWorldNL.Umiko.DDD;
using MadWorldNL.Umiko.ServiceBus;

namespace MadWorldNL.Umiko.CurriculaVitae;

public sealed record CurriculumVitaeCreated : IDomainEvent, IEvent
{
    public required Guid Id { get; init; }
    public required FullName FullName { get; init; }
    
    public DateTime OccurredOn { get; init; }
}