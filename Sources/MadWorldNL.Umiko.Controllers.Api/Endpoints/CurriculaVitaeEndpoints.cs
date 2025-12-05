using MadWorldNL.Umiko.CurriculaVitae;
using Microsoft.AspNetCore.Mvc;

namespace MadWorldNL.Umiko.Endpoints;

public static class CurriculaVitaeEndpoints
{
    public static void AddCurriculaVitaeEndpoints(this WebApplication app)
    {
        var curriculumVitaeBuilder = app.MapGroup("CurriculumVitae")
            .WithTags("CurriculumVitae");
        
        curriculumVitaeBuilder.MapPost("/", async ([FromServices] CreateCurriculumVitaeUseCase useCase) =>
        {
            var id = await useCase.Create();

            return new ObjectChangedResponse(id.Value);
        });
    }
}