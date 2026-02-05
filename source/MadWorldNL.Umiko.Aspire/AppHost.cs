using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var postgresDb = CreateDatabaseResource();

var keycloak = builder.AddKeycloak("keycloak")
    .WithDataVolume();

var rabbitmq = CreateMessagingResource();

var api = builder.AddProject<Api>("Api")
    .WaitFor(keycloak)
    .WaitFor(postgresDb)
    .WaitFor(rabbitmq)
    .WithReference(keycloak)
    .WithReference(postgresDb)
    .WithReference(rabbitmq)
    .WithHttpHealthCheck("/health");

var bus = builder.AddProject<Bus>("Bus")
    .WaitFor(keycloak)
    .WaitFor(postgresDb)
    .WaitFor(rabbitmq)
    .WithReference(keycloak)
    .WithReference(postgresDb)
    .WithReference(rabbitmq)
    .WithHttpHealthCheck("/health");

builder.AddProject<Web_Administrators>("Web-Administrators")
    .WithExternalHttpEndpoints()
    .WaitFor(api)
    .WaitFor(bus)
    .WaitFor(keycloak)
    .WithReference(api)
    .WithReference(bus)
    .WithReference(keycloak)
    .WithHttpHealthCheck("/health.txt");

builder.AddProject<Web_Users>("Web-Users")
    .WithExternalHttpEndpoints()
    .WaitFor(api)
    .WaitFor(bus)
    .WaitFor(keycloak)
    .WithReference(api)
    .WithReference(bus)
    .WithReference(keycloak)
    .WithHttpHealthCheck("/health.txt");

await builder.Build().RunAsync();

return;

IResourceBuilder<PostgresDatabaseResource> CreateDatabaseResource()
{
    var username = builder.AddParameter("Database-Username", secret: true);
    var password = builder.AddParameter("Database-Password", secret: true);

    var postgresServer = builder
        .AddPostgres("Postgres", username, password)
        .WithDataVolume(isReadOnly: false)
        .WithPgAdmin();

    var postgresDatabase = postgresServer.AddDatabase("PostgresDb");
    return postgresDatabase;
}

IResourceBuilder<RabbitMQServerResource> CreateMessagingResource()
{
    var username = builder.AddParameter("Messaging-Username", secret: true);
    var password = builder.AddParameter("Messaging-Password", secret: true);

    return builder.AddRabbitMQ("Messaging", username, password)
        .WithDataVolume(isReadOnly: false)
        .WithManagementPlugin();
}