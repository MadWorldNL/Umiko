using MadWorldNL.Umiko.DomainDrivenDevelopment.DefaultEntities;
using MadWorldNL.Umiko.Events;

namespace MadWorldNL.Umiko.CurriculaVitae;

public record NewCurriculumVitaeEvent(UniqueId Id) : IDomainEvent;