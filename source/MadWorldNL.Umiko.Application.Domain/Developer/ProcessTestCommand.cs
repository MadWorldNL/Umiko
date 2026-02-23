using MadWorldNL.Umiko.ServiceBus;

namespace MadWorldNL.Umiko.Developer;

public record ProcessTestCommand(string Message) : ICommand;