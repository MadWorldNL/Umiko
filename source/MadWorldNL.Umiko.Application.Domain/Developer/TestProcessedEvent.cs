using MadWorldNL.Umiko.ServiceBus;

namespace MadWorldNL.Umiko.Developer;

public record TestProcessedEvent(string Message) : IEvent;