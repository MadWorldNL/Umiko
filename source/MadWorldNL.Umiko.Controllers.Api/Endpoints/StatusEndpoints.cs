using MadWorldNL.Umiko.ServiceBus;
using MadWorldNL.Umiko.Status;

namespace MadWorldNL.Umiko.Endpoints;

internal static class StatusEndpoints
{
    internal static void AddStatusEndpoints(this WebApplication app)
    {
        var statusEndpoint = app.MapGroup("Status");

        statusEndpoint.MapGet("/Ping", () => "Pong")
            .WithName("Ping");

        statusEndpoint.MapGet("/Database", async (IQueryHandler<GetDatabaseStatusQuery, bool> handler, CancellationToken cancellationToken) =>
        {
            var result = await handler.Handle(new GetDatabaseStatusQuery(), cancellationToken);
            return result.Match(
                success: isConnected => new GetDatabaseStatusResponse { IsConnected = isConnected },
                failure: _ => new GetDatabaseStatusResponse { IsConnected = false });
        }).WithName("Database");
    }
}