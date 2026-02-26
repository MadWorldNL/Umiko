using MadWorldNL.Umiko.Functional;
using MadWorldNL.Umiko.Repositories;
using MadWorldNL.Umiko.ServiceBus;

namespace MadWorldNL.Umiko.CurriculaVitae;

public sealed class GetCurriculumVitaeFunction(IEventRepository<CurriculumVitae, Guid> repository)
    : IQueryHandler<GetCurriculumVitaeQuery, GetCurriculumVitaeResult>
{
    public async Task<Result<GetCurriculumVitaeResult>> Handle(GetCurriculumVitaeQuery query, CancellationToken cancellationToken)
    {
        var option = await repository.LoadAsync(query.Id, cancellationToken);

        return option.Match(
            some: cv => Result<GetCurriculumVitaeResult>.Success(
                new GetCurriculumVitaeResult(cv.Id, cv.FullName.FirstName, cv.FullName.LastName)),
            none: () => Result<GetCurriculumVitaeResult>.Failure(
                new KeyNotFoundException($"CurriculumVitae {query.Id} not found")));
    }
}