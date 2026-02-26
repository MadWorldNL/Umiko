using MadWorldNL.Umiko.ServiceBus;
using MadWorldNL.Umiko.Status;

namespace MadWorldNL.Umiko.Endpoints;

internal static class StatusEndpoints
{
    internal static void AddStatusEndpoints(this WebApplication app)
    {
        var statusEndpoint = app.MapGroup("Status")
            .WithGroupName("Status");

        statusEndpoint.MapGet("/Ping", () => "Pong")
            .WithName("Ping");

        statusEndpoint.MapGet("/Database", async (IQueryHandler<GetDatabaseStatusQuery, GetDatabaseStatusResult> handler, CancellationToken cancellationToken) =>
        {
            var result = await handler.Handle(new GetDatabaseStatusQuery(), cancellationToken);
            return result.Match(
                success: status => new GetDatabaseStatusResponse { IsConnected = status.IsConnected },
                failure: _ => new GetDatabaseStatusResponse { IsConnected = false });
        }).WithName("Database");

        statusEndpoint.MapGet("/MessageBus", async (IQueryHandler<GetMessagingStatusQuery, GetMessagingStatusResult> handler, CancellationToken cancellationToken) =>
        {
            var result = await handler.Handle(new GetMessagingStatusQuery(), cancellationToken);
            return result.Match(
                success: status => new GetMessagingStatusResponse { IsConnected = status.IsConnected },
                failure: _ => new GetMessagingStatusResponse { IsConnected = false });
        }).WithName("MessageBus");
    }
}