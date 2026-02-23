using MadWorldNL.Umiko.ServiceBus;

namespace MadWorldNL.Umiko.Developer;

public sealed record ProcessTestCommand(string Message) : ICommand;