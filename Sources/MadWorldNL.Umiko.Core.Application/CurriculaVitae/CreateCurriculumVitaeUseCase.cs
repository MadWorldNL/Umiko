using MadWorldNL.Umiko.DomainDrivenDevelopment.DefaultEntities;
using MadWorldNL.Umiko.Events;

namespace MadWorldNL.Umiko.CurriculaVitae;

public class CreateCurriculumVitaeUseCase(IEventsContext eventsContext)
{
    public async Task<UniqueId> Create()
    {
        var curriculumVitae = CurriculumVitae.New();
        
        await eventsContext.Store(curriculumVitae);
        
        return curriculumVitae.Id;
    }
}