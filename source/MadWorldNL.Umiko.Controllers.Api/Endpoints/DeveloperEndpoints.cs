using MadWorldNL.Umiko.Developer;
using MadWorldNL.Umiko.ServiceBus;

namespace MadWorldNL.Umiko.Endpoints;

internal static class DeveloperEndpoints
{
    internal static void AddDeveloperEndpoints(this WebApplication app)
    {
        var developerEndpoint = app.MapGroup("Developer")
            .WithGroupName("Developer");

        developerEndpoint.MapPost("/Command", async (SendTestCommandRequest request, IMessageBus messageBus, CancellationToken cancellationToken) =>
        {
            if (string.IsNullOrWhiteSpace(request.Message))
            {
                return Results.BadRequest("Message cannot be empty.");
            }
            
            if (request.Message.Length > 100)
            {
                return Results.BadRequest("Message length exceeds maximum allowed characters (100).");
            }
            
            await messageBus.Send(new ProcessTestCommand(request.Message));
            return Results.Accepted();
        }).WithName("SendTestCommand");
    }
}