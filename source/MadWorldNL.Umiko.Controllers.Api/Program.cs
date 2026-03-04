using Microsoft.AspNetCore.HttpOverrides;
using MadWorldNL.Umiko;
using MadWorldNL.Umiko.Configurations;
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

builder.Services.AddHealthChecks();
builder.Services.AddRateLimiterPolicy();
builder.Services.AddValidation();
builder.AddDefaultAuthentication();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
});
builder.Services.AddPostgresqlServices();
builder.Services.AddRabbitMqServices();
builder.Services.AddFunctionsServices();

builder.Services.AddEventConsumer<TestProcessedEvent>();

var app = builder.Build();

app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = _ => false
}).DisableRateLimiting();

app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.AddStatusEndpoints();
app.AddDeveloperEndpoints();
app.AddCurriculaVitaeEndpoints();

await app.RunAsync();