using CopilotWeather.Api;
using CopilotWeather.Logic.Abstractions;
using CopilotWeather.Logic.Models;
using CopilotWeather.Logic.Providers;
using CopilotWeather.Logic.Services;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add OpenAPI services
builder.Services.AddOpenApi();

// Add problem details support
builder.Services.AddProblemDetails();

// Register weather services
builder.Services.AddScoped<IWeatherProvider, MockWeatherProvider>();
builder.Services.AddScoped<IWeatherService, WeatherService>();

var app = builder.Build();

// Configure OpenAPI and Scalar UI
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options => options
        .WithTitle("CopilotWeather API")
        .WithTheme(ScalarTheme.BluePlanet)
        .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient)
        .WithEndpointPrefix("/docs/{documentName}")
    );
}

// Add problem details middleware
app.UseHttpsRedirection();

// Configure weather endpoints
app.MapWeatherEndpoints();

app.Run();
