namespace MadWorldNL.Umiko.DDD;

public interface IDomainEvent
{
    DateTime OccurredOn { get; }
}