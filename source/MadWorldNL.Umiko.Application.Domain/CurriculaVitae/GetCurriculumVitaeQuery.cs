using MadWorldNL.Umiko.ServiceBus;

namespace MadWorldNL.Umiko.CurriculaVitae;

public sealed record GetCurriculumVitaeQuery(Guid Id) : IQuery<GetCurriculumVitaeResult>;