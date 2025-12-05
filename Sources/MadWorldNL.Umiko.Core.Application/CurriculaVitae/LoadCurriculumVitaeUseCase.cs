using LanguageExt;
using MadWorldNL.Umiko.Events;

namespace MadWorldNL.Umiko.CurriculaVitae;

public class LoadCurriculumVitaeUseCase(IEventsContext eventsContext)
{
    public async Task<Option<CurriculumVitae>> Load(Guid id)
    {
        return await eventsContext.GetById<CurriculumVitae>(id);
    }
}