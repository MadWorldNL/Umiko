using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var api = builder.AddProject<Api>("Api")
    .WithHttpHealthCheck("/health");

builder.Build().Run();