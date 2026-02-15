namespace MadWorldNL.Umiko.Status;

public interface IStatusRepository
{
    Task<bool> CanConnect(CancellationToken cancellationToken = default);
}