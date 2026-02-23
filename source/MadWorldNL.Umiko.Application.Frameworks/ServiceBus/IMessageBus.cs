namespace MadWorldNL.Umiko.ServiceBus;

public interface IMessageBus
{
    Task Send<TCommand>(TCommand command) where TCommand : ICommand;
    Task Publish<TEvent>(TEvent[] events) where TEvent : IEvent;
}