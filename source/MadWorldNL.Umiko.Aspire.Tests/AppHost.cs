using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var postgresDb = builder
    .AddPostgres("Postgres")
    .AddDatabase("PostgresDb");

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

builder.Build().Run();