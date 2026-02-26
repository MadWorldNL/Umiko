using MadWorldNL.Umiko.Functional;
using MadWorldNL.Umiko.Repositories;
using MadWorldNL.Umiko.ServiceBus;

namespace MadWorldNL.Umiko.CurriculaVitae;

public sealed class CreateCurriculumVitaeFunction(
    IEventRepository<CurriculumVitae, Guid> repository,
    IMessageBus messageBus) : ICommandHandler<CreateCurriculumVitaeCommand>
{
    public async Task<Result<bool>> Handle(CreateCurriculumVitaeCommand command, CancellationToken cancellationToken)
    {
        var cv = CurriculumVitae.Create(new FullName(command.FirstName, command.LastName));

        await repository.SaveAsync(cv, cancellationToken);

        await messageBus.Publish([new CurriculumVitaeCreated
        {
            Id = cv.Id,
            FullName = cv.FullName,
            OccurredOn = DateTime.UtcNow
        }]);

        return Result<bool>.Success(true);
    }
}