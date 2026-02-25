namespace MadWorldNL.Umiko.Helpers.DDD;

public record TestDomainEvent(DateTime OccurredOn) : IDomainEvent;