using MadWorldNL.Umiko.ServiceBus;

namespace MadWorldNL.Umiko.CurriculaVitae;

public sealed record CreateCurriculumVitaeCommand(Guid Id, string FirstName, string LastName) : ICommand;