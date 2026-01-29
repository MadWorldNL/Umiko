using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("Postgres");
var postgresDb = postgres.AddDatabase("PostgresDb");

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

IResourceBuilder<RabbitMQServerResource> CreateMessagingResource()
{
    var username = builder.AddParameter("Messaging-Username", secret: true);
    var password = builder.AddParameter("Messaging-Password", secret: true);

    return builder.AddRabbitMQ("Messaging", username, password)
        .WithDataVolume(isReadOnly: false)
        .WithManagementPlugin();
}