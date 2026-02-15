namespace MadWorldNL.Umiko.Status;

public class StatusRepository(UmikoContext context) : IStatusRepository
{
    public async Task<bool> CanConnect(CancellationToken cancellationToken = default)
    {
        return await context.Database.CanConnectAsync(cancellationToken);
    }
}