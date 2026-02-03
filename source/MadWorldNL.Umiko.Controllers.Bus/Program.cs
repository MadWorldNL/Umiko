using MadWorldNL.Umiko.Configurations;
using MadWorldNL.Umiko.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.AddOpenTelemetry();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapHealthChecks("/health");
app.AddStatusEndpoints();

app.Run();
