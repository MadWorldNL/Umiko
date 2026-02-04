using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var postgresDb = builder
    .AddPostgres("Postgres")
    .WithImageTag("latest")
    .AddDatabase("PostgresDb");

var rabbitmq = builder.AddRabbitMQ("Messaging")
    .WithImageTag("latest");

var api = builder.AddProject<Api>("Api")
    .WaitFor(postgresDb)
    .WaitFor(rabbitmq)
    .WithReference(postgresDb)
    .WithReference(rabbitmq)
    .WithHttpHealthCheck("/health");

builder.Build().Run();