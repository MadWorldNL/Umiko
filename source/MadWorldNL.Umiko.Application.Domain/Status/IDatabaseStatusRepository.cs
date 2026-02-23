namespace MadWorldNL.Umiko.Status;

public interface IDatabaseStatusRepository
{
    Task<bool> CanConnect(CancellationToken cancellationToken = default);
}