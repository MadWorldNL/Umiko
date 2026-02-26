using MadWorldNL.Umiko.ServiceBus;

namespace MadWorldNL.Umiko.CurriculaVitae;

public sealed record CreateCurriculumVitaeCommand(string FirstName, string LastName) : ICommand;