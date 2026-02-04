using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var postgresDb = builder
    .AddPostgres("Postgres")
    .AddDatabase("PostgresDb");

var rabbitmq = builder.AddRabbitMQ("Messaging");

var api = builder.AddProject<Api>("Api")
    .WithReference(postgresDb)
    .WithReference(rabbitmq)
    .WithHttpHealthCheck("/health");

builder.Build().Run();