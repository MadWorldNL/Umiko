using MadWorldNL.Umiko;
using MadWorldNL.Umiko.Configurations;
using MadWorldNL.Umiko.Endpoints;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddOpenTelemetry();
builder.AddNpgsqlDbContext<UmikoContext>("UmikoDb");

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddHealthChecks();
builder.Services.AddPostgresqlServices();
builder.Services.AddFunctionsServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.MapHealthChecks("/health");
app.AddStatusEndpoints();

await app.RunAsync();