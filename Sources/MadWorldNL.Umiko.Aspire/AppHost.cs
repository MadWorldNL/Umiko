using MadWorldNL.Umiko;

var builder = DistributedApplication.CreateBuilder(args);

_ = builder.AddKubernetesEnvironment("k8s");

var username = builder.AddParameter("db-username", secret: true);
var password = builder.AddParameter("db-password", secret: true);

var postgres = builder.AddPostgres(ContainerDefinitions.Database, username, password)
    .WithVolume(
        name: "DbData",
        target: "/PostgreSQL/Data",
        isReadOnly: false)
    .WithPgAdmin(pgAdmin => pgAdmin.WithHostPort(5050))
    .WithChildRelationship(username)
    .WithChildRelationship(password);
    
var umikoDb = postgres.AddDatabase("umikodb");

var api = builder.AddProject<Projects.Api>(ProjectDefinitions.Api)
    .WithUrlForEndpoint("http", c => c.DisplayText = "ApiInsecure")
    .WithUrlForEndpoint("https", c => c.DisplayText = "ApiSecure")
    .WithExternalHttpEndpoints()
    .WithReference(umikoDb)
    .WaitFor(umikoDb);

builder.AddProject<Projects.Web>(ProjectDefinitions.Web)
    .WithUrlForEndpoint("http", c => c.DisplayText = "WebInsecure")
    .WithUrlForEndpoint("https", c => c.DisplayText = "WebSecure")
    .WithExternalHttpEndpoints()
    .WaitFor(api);

builder.Build().Run();