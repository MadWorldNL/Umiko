using MadWorldNL.Umiko;

var builder = DistributedApplication.CreateBuilder(args);

var api = builder.AddProject<Projects.Api>(ProjectDefinitions.Api)
    .WithUrlForEndpoint("http", c => c.DisplayText = "ApiInsecure")
    .WithUrlForEndpoint("https", c => c.DisplayText = "ApiSecure")
    .WithExternalHttpEndpoints();

builder.AddProject<Projects.Web>(ProjectDefinitions.Web)
    .WithUrlForEndpoint("http", c => c.DisplayText = "WebInsecure")
    .WithUrlForEndpoint("https", c => c.DisplayText = "WebSecure")
    .WithExternalHttpEndpoints()
    .WaitFor(api);

builder.Build().Run();