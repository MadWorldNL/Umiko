using MadWorldNL.Umiko.Configurations;
using MadWorldNL.Umiko.Endpoints;
using MadWorldNL.Umiko.Endpoints.DebugTools;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.AddApplication();
builder.AddDatabase();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
    
    app.AddDebugToolsEndpoints();
}

app.AddCurriculaVitaeEndpoints();

app.UseHttpsRedirection();

app.Run();