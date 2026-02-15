using MadWorldNL.Umiko.Status;

namespace MadWorldNL.Umiko.Endpoints;

internal static class StatusEndpoints
{
    internal static void AddStatusEndpoints(this WebApplication app)
    {
        var statusEndpoint = app.MapGroup("Status");

        statusEndpoint.MapGet("/Ping", () => "Pong")
            .WithName("Ping");

        statusEndpoint.MapGet("/Database", async (GetDatabaseStatusFunction function, CancellationToken cancellationToken) =>
        {
            var isConnected = await function.ExecuteAsync(cancellationToken);
            return new GetDatabaseStatusResponse { IsConnected = isConnected };
        }).WithName("Database");
    }
}