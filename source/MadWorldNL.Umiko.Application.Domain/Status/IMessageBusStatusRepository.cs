namespace MadWorldNL.Umiko.Status;

public interface IMessageBusStatusRepository
{
    Task<bool> CanConnect(CancellationToken cancellationToken = default);
}
