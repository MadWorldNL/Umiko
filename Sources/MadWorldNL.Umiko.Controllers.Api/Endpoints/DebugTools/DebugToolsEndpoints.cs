using System.Collections;

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
    }
}