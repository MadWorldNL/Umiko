using MadWorldNL.Umiko.ServiceBus;

namespace MadWorldNL.Umiko.Status;

public sealed record GetDatabaseStatusQuery : IQuery<GetDatabaseStatusResult>;