using RabbitMQ.Client;

namespace MadWorldNL.Umiko.Status;

public sealed class MessageBusStatusRepository(IConnection connection) : IMessageBusStatusRepository
{
    public Task<bool> CanConnect(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(connection.IsOpen);
    }
}