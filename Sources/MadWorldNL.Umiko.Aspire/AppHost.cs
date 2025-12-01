using System.Net.Sockets;
using MadWorldNL.Umiko;

var builder = DistributedApplication.CreateBuilder(args);

var database = builder.AddContainer(ContainerDefinitions.Database, "postgres:18")
    .WithEnvironment("POSTGRES_PASSWORD", "SecretP@ssw0rd")
    .WithVolume(target: "/var/lib/postgresql")
    .WithEndpoint(5432, 5432);

builder.AddContainer(ContainerDefinitions.PgAdmin, "dpage/pgadmin4:9")
    .WithEnvironment("PGADMIN_DEFAULT_EMAIL", "test@test.nl")
    .WithEnvironment("PGADMIN_DEFAULT_PASSWORD", "Secret1234")
    .WithVolume(target: "/var/lib/pgadmin")
    .WithEndpoint("http", e => {
        e.Port = 9080;
        e.TargetPort = 80;
        e.Protocol = ProtocolType.Tcp;
        e.UriScheme = "http";
    })
    .WithUrlForEndpoint("http", c => c.DisplayText = "PgAdminInsecure")
    .WaitFor(database);

var api = builder.AddProject<Projects.Api>(ProjectDefinitions.Api)
    .WithUrlForEndpoint("http", c => c.DisplayText = "ApiInsecure")
    .WithUrlForEndpoint("https", c => c.DisplayText = "ApiSecure")
    .WithExternalHttpEndpoints()
    .WaitFor(database);

builder.AddProject<Projects.Web>(ProjectDefinitions.Web)
    .WithUrlForEndpoint("http", c => c.DisplayText = "WebInsecure")
    .WithUrlForEndpoint("https", c => c.DisplayText = "WebSecure")
    .WithExternalHttpEndpoints()
    .WaitFor(api);

builder.Build().Run();