using MadWorldNL.Umiko.ServiceBus;

namespace MadWorldNL.Umiko.Developer;

public sealed record TestProcessedEvent(string Message) : IEvent;