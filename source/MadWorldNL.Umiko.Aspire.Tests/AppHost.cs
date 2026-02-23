using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var postgresDb = builder
    .AddPostgres("Postgres")
    .AddDatabase("UmikoDb");

var rabbitmq = builder.AddRabbitMQ("UmikoBus");

var api = builder.AddProject<Api>("Api")
    .WaitFor(postgresDb)
    .WaitFor(rabbitmq)
    .WithReference(postgresDb)
    .WithReference(rabbitmq)
    .WithEnvironment("RateLimiter__PermitLimit", "5")
    .WithHttpHealthCheck("/health");

var bus = builder.AddProject<Bus>("Bus")
    .WaitFor(postgresDb)
    .WaitFor(rabbitmq)
    .WithReference(postgresDb)
    .WithReference(rabbitmq)
    .WithEnvironment("RateLimiter__PermitLimit", "5")
    .WithHttpHealthCheck("/health");

builder.AddProject<Web_Administrators>("Web-Administrators")
    .WithExternalHttpEndpoints()
    .WithDeveloperCertificateTrust(trust: true)
    .WithEndpoint("https", endpoint => endpoint.IsExternal = false)
    .WaitFor(api)
    .WaitFor(bus)
    .WithReference(api)
    .WithReference(bus)
    .WithHttpHealthCheck("/health.txt");

builder.AddProject<Web_Users>("Web-Users")
    .WithExternalHttpEndpoints()
    .WithDeveloperCertificateTrust(trust: true)
    .WithEndpoint("https", endpoint => endpoint.IsExternal = false)
    .WaitFor(api)
    .WaitFor(bus)
    .WithReference(api)
    .WithReference(bus)
    .WithHttpHealthCheck("/health.txt");

await builder.Build().RunAsync();