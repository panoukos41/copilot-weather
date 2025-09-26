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

// Weather endpoints
app.MapGet("/weather/{location}", async (string location, IWeatherService weatherService, string? provider = null) =>
{
    var request = new WeatherRequest 
    { 
        Location = location,
        Provider = provider 
    };
    var result = await weatherService.GetWeatherAsync(request);
    
    if (!result.IsSuccess)
    {
        return Results.Problem(
            detail: result.Error,
            statusCode: 400,
            title: "Invalid Weather Provider"
        );
    }
    
    return Results.Ok(result.Value);
})
.WithName("GetWeather")
.WithSummary("Get weather for a specific location")
.WithDescription("Retrieves weather data for the specified location, optionally from a specific provider")
.WithTags("Weather");

app.MapGet("/weather/{location}/all-providers", async (string location, IWeatherService weatherService) =>
{
    var weather = await weatherService.GetWeatherFromAllProvidersAsync(location);
    return Results.Ok(weather);
})
.WithName("GetWeatherFromAllProviders")
.WithSummary("Get weather from all available providers")
.WithDescription("Retrieves weather data from all configured providers for comparison")
.WithTags("Weather");

app.MapGet("/weather/{location}/aggregated", async (string location, IWeatherService weatherService) =>
{
    var weather = await weatherService.GetAggregatedWeatherAsync(location);
    return Results.Ok(weather);
})
.WithName("GetAggregatedWeather")
.WithSummary("Get aggregated weather data from all providers")
.WithDescription("Retrieves weather data aggregated from all available providers")
.WithTags("Weather");

// Get available providers
app.MapGet("/providers", (IWeatherService weatherService) =>
{
    var providers = weatherService.GetAvailableProviders();
    return Results.Ok(providers);
})
.WithName("GetProviders")
.WithSummary("Get list of available weather providers")
.WithDescription("Returns a list of all configured weather data providers")
.WithTags("Providers");

app.Run();
