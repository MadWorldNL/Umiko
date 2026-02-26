using MadWorldNL.Umiko.CurriculaVitae;
using MadWorldNL.Umiko.ServiceBus;

namespace MadWorldNL.Umiko.Endpoints;

internal static class CurriculaVitaeEndpoints
{
    internal static void AddCurriculaVitaeEndpoints(this WebApplication app)
    {
        var curriculaVitaeEndpoint = app.MapGroup("CurriculaVitae");

        curriculaVitaeEndpoint.MapPost("/", async (CreateCurriculumVitaeRequest request, IMessageBus messageBus, CancellationToken cancellationToken) =>
        {
            await messageBus.Send(new CreateCurriculumVitaeCommand(request.FirstName, request.LastName));
            return Results.Accepted();
        }).WithName("CreateCurriculumVitae");

        curriculaVitaeEndpoint.MapGet("/{id:guid}", async (Guid id, IQueryHandler<GetCurriculumVitaeQuery, GetCurriculumVitaeResult> handler, CancellationToken cancellationToken) =>
        {
            var result = await handler.Handle(new GetCurriculumVitaeQuery(id), cancellationToken);
            return result.Match(
                success: cv => Results.Ok(new GetCurriculumVitaeResponse
                {
                    Id = cv.Id,
                    FirstName = cv.FirstName,
                    LastName = cv.LastName
                }),
                failure: _ => Results.NotFound());
        }).WithName("GetCurriculumVitae");
    }
}