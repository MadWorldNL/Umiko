using RabbitMQ.Client;

namespace MadWorldNL.Umiko.Status;

public class MessageBusStatusRepository(IConnection connection) : IMessageBusStatusRepository
{
    public Task<bool> CanConnect(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(connection.IsOpen);
    }
}