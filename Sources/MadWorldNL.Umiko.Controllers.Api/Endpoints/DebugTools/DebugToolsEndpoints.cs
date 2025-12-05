using System.Collections;
using MadWorldNL.Umiko.CurriculaVitae;
using MadWorldNL.Umiko.Events;
using Microsoft.AspNetCore.Mvc;

namespace MadWorldNL.Umiko.Endpoints.DebugTools;

public static class DebugToolsEndpoints
{
    /// <summary>
    /// Registers developer-only diagnostic endpoints used for inspecting
    /// runtime details such as environment variables.  
    /// These tools are intended **only for use in Debug mode** and should not
    /// be exposed in production environments.
    /// </summary>
    /// <param name="app"></param>
    public static void AddDebugToolsEndpoints(this WebApplication app)
    {
        var debugBuilder = app.MapGroup("Debug")
            .WithTags("Debug");
        
        debugBuilder.MapGet("EnvironmentVariables", () =>
        {
            Dictionary<string, string> environmentVariables = [];
            foreach (DictionaryEntry environmentVariable in Environment.GetEnvironmentVariables())
            {
                environmentVariables.Add((string)environmentVariable.Key, environmentVariable.Value?.ToString() ?? string.Empty);
            }
            
            return new GetEnvironmentsVariablesResponse()
            {
                EnvironmentVariables = environmentVariables
            };
        });

        debugBuilder.MapPost("CV", async ([FromServices] IEventsContext eventsContext) =>
        {
            var cv = CurriculumVitae.New();

            await eventsContext.Store(cv);
            
            return cv.Id.Value;
        });

        debugBuilder.MapGet("CV/{id}", async ([FromRoute] Guid id, [FromServices] IEventsContext eventsContext) =>
        {
            var cv = await eventsContext.GetById<CurriculumVitae>(id);
            return cv.Match(
                Results.Ok, 
                () => Results.NotFound());
        });
    }
}