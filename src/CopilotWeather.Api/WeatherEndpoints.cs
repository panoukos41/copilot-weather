using CopilotWeather.Logic.Abstractions;
using CopilotWeather.Logic.Models;

namespace CopilotWeather.Api;

public static class WeatherEndpoints
{
    public static void MapWeatherEndpoints(this IEndpointRouteBuilder app)
    {
        // Weather Group
        var weatherGroup = app
            .MapGroup("/weather")
            .WithTags("Weather");

        // Get weather for a specific location
        weatherGroup.MapGet("/{location}", GetWeatherAsync)
            .WithName("GetWeather")
            .WithSummary("Get weather for a specific location")
            .WithDescription("Retrieves weather data for the specified location, optionally from a specific provider");

        // Get weather from all providers
        weatherGroup.MapGet("/{location}/all-providers", GetWeatherFromAllProvidersAsync)
            .WithName("GetWeatherFromAllProviders")
            .WithSummary("Get weather from all available providers")
            .WithDescription("Retrieves weather data from all configured providers for comparison");

        // Get aggregated weather data
        weatherGroup.MapGet("/{location}/aggregated", GetAggregatedWeatherAsync)
            .WithName("GetAggregatedWeather")
            .WithSummary("Get aggregated weather data from all providers")
            .WithDescription("Retrieves weather data aggregated from all available providers");

        // Get available providers
        weatherGroup.MapGet("/providers", GetProviders)
            .WithName("GetProviders")
            .WithSummary("Get list of available weather providers")
            .WithDescription("Returns a list of all configured weather data providers");
    }

    private static async Task<IResult> GetWeatherAsync(string location, IWeatherService weatherService, string? provider = null)
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
    }

    private static async Task<IResult> GetWeatherFromAllProvidersAsync(string location, IWeatherService weatherService)
    {
        var weather = await weatherService.GetWeatherFromAllProvidersAsync(location);
        return Results.Ok(weather);
    }

    private static async Task<IResult> GetAggregatedWeatherAsync(string location, IWeatherService weatherService)
    {
        var weather = await weatherService.GetAggregatedWeatherAsync(location);
        return Results.Ok(weather);
    }

    private static IResult GetProviders(IWeatherService weatherService)
    {
        var providers = weatherService.GetAvailableProviders();
        return Results.Ok(providers);
    }
}