namespace MadWorldNL.Umiko.Tests.Helpers.DDD;

public record TestDomainEvent(DateTime OccurredOn) : IDomainEvent;