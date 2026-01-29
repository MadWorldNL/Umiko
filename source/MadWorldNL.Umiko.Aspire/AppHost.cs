using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var postgresDb = CreateDatabaseResource();
var rabbitmq = CreateMessagingResource();

var api = builder.AddProject<Api>("Api")
    .WaitFor(postgresDb)
    .WaitFor(rabbitmq)
    .WithReference(postgresDb)
    .WithReference(rabbitmq);

var bus = builder.AddProject<Bus>("Bus")
    .WaitFor(postgresDb)
    .WaitFor(rabbitmq)
    .WithReference(postgresDb)
    .WithReference(rabbitmq);

builder.AddProject<Web_Administrators>("Web-Administrators")
    .WaitFor(api)
    .WithReference(api)
    .WaitFor(bus)
    .WithReference(bus);

builder.AddProject<Web_Users>("Web-Users")
    .WaitFor(api)
    .WithReference(api)
    .WaitFor(bus)
    .WithReference(bus);

builder.Build().Run();

return;

IResourceBuilder<PostgresDatabaseResource> CreateDatabaseResource()
{
    var username = builder.AddParameter("Database-Username", secret: true);
    var password = builder.AddParameter("Database-Password", secret: true);

    var postgres = builder.AddPostgres("Postgres", username, password)
        .WithPgAdmin();

    return postgres.AddDatabase("PostgresDb");
}

IResourceBuilder<RabbitMQServerResource> CreateMessagingResource()
{
    var username = builder.AddParameter("Messaging-Username", secret: true);
    var password = builder.AddParameter("Messaging-Password", secret: true);

    return builder.AddRabbitMQ("Messaging", username, password)
        .WithDataVolume(isReadOnly: false)
        .WithManagementPlugin();
}