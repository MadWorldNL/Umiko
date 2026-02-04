using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var api = builder.AddProject<Api>("Api");

builder.Build().Run();