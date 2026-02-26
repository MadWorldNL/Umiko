using Microsoft.AspNetCore.HttpOverrides;
using MadWorldNL.Umiko;
using MadWorldNL.Umiko.Configurations;
using MadWorldNL.Umiko.CurriculaVitae;
using MadWorldNL.Umiko.Developer;
using MadWorldNL.Umiko.Endpoints;
using Marten;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddOpenTelemetry();
builder.AddNpgsqlDbContext<UmikoContext>("UmikoDb");
builder.AddRabbitMQClient("UmikoBus");
builder.Services.AddMarten(options =>
{
    options.Connection(builder.Configuration.GetConnectionString("UmikoDb")!);
    options.Events.DatabaseSchemaName = "marten";
    options.DatabaseSchemaName = "marten";
}).UseLightweightSessions();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddHealthChecks();
builder.Services.AddRateLimiterPolicy();
builder.Services.AddPostgresqlServices();
builder.Services.AddRabbitMqServices();
builder.Services.AddFunctionsServices();

builder.Services.AddCommandConsumer<ProcessTestCommand>();
builder.Services.AddCommandConsumer<CreateCurriculumVitaeCommand>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseRateLimiter();
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = _ => false
}).DisableRateLimiting();
app.AddStatusEndpoints();

await app.RunAsync();
