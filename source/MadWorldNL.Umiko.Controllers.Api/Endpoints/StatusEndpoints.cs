namespace MadWorldNL.Umiko.Endpoints;

internal static class StatusEndpoints
{
    internal static void AddStatusEndpoints(this WebApplication app)
    {
        var statusEndpoint = app.MapGroup("Status");

        statusEndpoint.MapGet("/Ping", () => "Pong")
            .WithName("Ping");
    }
}