using MadWorldNL.Umiko.Developer;
using MadWorldNL.Umiko.ServiceBus;

namespace MadWorldNL.Umiko.Endpoints;

internal static class DeveloperEndpoints
{
    internal static void AddDeveloperEndpoints(this WebApplication app)
    {
        var developerEndpoint = app.MapGroup("Developer");

        developerEndpoint.MapPost("/Command", async (SendTestCommandRequest request, IMessageBus messageBus, CancellationToken cancellationToken) =>
        {
            await messageBus.Send(new ProcessTestCommand(request.Message));
            return Results.Accepted();
        }).WithName("SendTestCommand");
    }
}