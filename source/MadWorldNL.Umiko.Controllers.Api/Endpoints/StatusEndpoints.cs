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

        statusEndpoint.MapGet("/Database", async (IQueryHandler<GetDatabaseStatusQuery, GetDatabaseStatusResult> handler, CancellationToken cancellationToken) =>
        {
            var result = await handler.Handle(new GetDatabaseStatusQuery(), cancellationToken);
            return result.Match(
                success: status => new GetDatabaseStatusResponse { IsConnected = status.IsConnected },
                failure: _ => new GetDatabaseStatusResponse { IsConnected = false });
        }).WithName("Database");
    }
}