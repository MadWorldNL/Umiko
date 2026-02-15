namespace MadWorldNL.Umiko.Status;

public class GetDatabaseStatusFunction(IStatusRepository statusRepository)
{
    public async Task<bool> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        return await statusRepository.CanConnect(cancellationToken);
    }
}