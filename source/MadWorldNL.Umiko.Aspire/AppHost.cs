using Projects;

var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Api>("Api");

builder.AddProject<Web_Administrators>("Web-Administrators");

builder.AddProject<Web_Users>("Web-Users");

builder.Build().Run();