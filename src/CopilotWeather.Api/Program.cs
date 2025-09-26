using CopilotWeather.Logic.Abstractions;
using CopilotWeather.Logic.Models;
using CopilotWeather.Logic.Providers;
using CopilotWeather.Logic.Services;

var builder = WebApplication.CreateBuilder(args);

// Register weather services
builder.Services.AddScoped<IWeatherProvider, MockWeatherProvider>();
builder.Services.AddScoped<IWeatherService, WeatherService>();

var app = builder.Build();

app.UseHttpsRedirection();

// Weather endpoints
app.MapGet("/weather/{location}", async (string location, IWeatherService weatherService) =>
{
    var request = new WeatherRequest { Location = location };
    var weather = await weatherService.GetWeatherAsync(request);
    return Results.Ok(weather);
})
.WithName("GetWeather")
.WithSummary("Get weather for a specific location");

app.MapGet("/weather/{location}/all-providers", async (string location, IWeatherService weatherService) =>
{
    var request = new WeatherRequest { Location = location };
    var weather = await weatherService.GetWeatherFromAllProvidersAsync(request);
    return Results.Ok(weather);
})
.WithName("GetWeatherFromAllProviders")
.WithSummary("Get weather from all available providers");

app.MapGet("/weather/{location}/aggregated", async (string location, IWeatherService weatherService) =>
{
    var request = new WeatherRequest { Location = location };
    var weather = await weatherService.GetAggregatedWeatherAsync(request);
    return Results.Ok(weather);
})
.WithName("GetAggregatedWeather")
.WithSummary("Get aggregated weather data from all providers");

// Health check endpoint
app.MapGet("/health", () => Results.Ok(new { Status = "Healthy", Timestamp = DateTime.UtcNow }));

app.Run();
