using MadWorldNL.Umiko.Functional;

namespace MadWorldNL.Umiko.ServiceBus;

public interface IEventHandler<in TEvent>
    where TEvent : IEvent
{
    Task<Result<bool>> Handle(TEvent @event, CancellationToken cancellationToken);
}