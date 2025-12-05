using JetBrains.Annotations;
using MadWorldNL.Umiko.DomainDrivenDevelopment;
using MadWorldNL.Umiko.DomainDrivenDevelopment.DefaultEntities;
using MadWorldNL.Umiko.Events;

namespace MadWorldNL.Umiko.CurriculaVitae;

public class CurriculumVitae : RootAggregate
{
    public override string AggregateType => nameof(CurriculumVitae);
    
    [UsedImplicitly]
    private CurriculumVitae()
    {
    }

    private CurriculumVitae(string name)
    {
        Id = new UniqueId(Guid.NewGuid());
        
        Raise(new NewCurriculumVitaeEvent(Id));
    }

    public static CurriculumVitae New()
    {
        return new CurriculumVitae("");
    }
    
    public override void Apply(IDomainEvent @event)
    {
        switch (@event)
        {
            case NewCurriculumVitaeEvent newEvent: 
                Apply(newEvent); 
                break;
        }
    }
    
    private void Apply(NewCurriculumVitaeEvent @event)
    {
        Id = @event.Id;
    }
}