namespace MadWorldNL.Umiko.Status;

public sealed class DatabaseStatusRepository(UmikoContext context) : IDatabaseStatusRepository
{
    public async Task<bool> CanConnect(CancellationToken cancellationToken = default)
    {
        return await context.Database.CanConnectAsync(cancellationToken);
    }
}