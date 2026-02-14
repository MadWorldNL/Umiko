using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var postgresDb = builder
    .AddPostgres("Postgres")
    .AddDatabase("UmikoDb");

var rabbitmq = builder.AddRabbitMQ("Messaging");

var api = builder.AddProject<Api>("Api")
    .WaitFor(postgresDb)
    .WaitFor(rabbitmq)
    .WithReference(postgresDb)
    .WithReference(rabbitmq)
    .WithHttpHealthCheck("/health");

var bus = builder.AddProject<Bus>("Bus")
    .WaitFor(postgresDb)
    .WaitFor(rabbitmq)
    .WithReference(postgresDb)
    .WithReference(rabbitmq)
    .WithHttpHealthCheck("/health");

builder.AddProject<Web_Administrators>("Web-Administrators")
    .WithExternalHttpEndpoints()
    .WithDeveloperCertificateTrust(trust: true)
    .WaitFor(api)
    .WaitFor(bus)
    .WithReference(api)
    .WithReference(bus)
    .WithHttpHealthCheck("/health.txt");

builder.AddProject<Web_Users>("Web-Users")
    .WithExternalHttpEndpoints()
    .WithDeveloperCertificateTrust(trust: true)
    .WaitFor(api)
    .WaitFor(bus)
    .WithReference(api)
    .WithReference(bus)
    .WithHttpHealthCheck("/health.txt");

await builder.Build().RunAsync();