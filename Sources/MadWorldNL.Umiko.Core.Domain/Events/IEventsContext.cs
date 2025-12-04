using LanguageExt;
using MadWorldNL.Umiko.DomainDrivenDevelopment;

namespace MadWorldNL.Umiko.Events;

public interface IEventsContext
{
    Task<Option<TRootAggregate>> GetById<TRootAggregate>(Guid id) where TRootAggregate : RootAggregate;
    Task Store<TRootAggregate>(TRootAggregate aggregate) where TRootAggregate : RootAggregate;
}