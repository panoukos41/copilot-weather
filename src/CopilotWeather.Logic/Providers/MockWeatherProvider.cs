using CopilotWeather.Logic.Abstractions;
using CopilotWeather.Logic.Models;

namespace CopilotWeather.Logic.Providers;

public class MockWeatherProvider : IWeatherProvider
{
    private readonly Random _random = new();
    
    public string Name => "Mock";

    public async Task<WeatherData> GetWeatherAsync(ProviderWeatherRequest request, CancellationToken cancellationToken = default)
    {
        // Simulate some processing time
        await Task.Delay(100, cancellationToken);
        
        var data = new WeatherData
        {
            Location = request.Location,
            Temperature = _random.Next(-10, 35),
            Humidity = _random.Next(30, 90),
            Description = GetRandomDescription(),
            Provider = Name,
            Timestamp = DateTime.UtcNow
        };

        return data;
    }

    public Task<bool> IsAvailableAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(true);
    }

    private string GetRandomDescription()
    {
        var descriptions = new[]
        {
            "Clear sky",
            "Partly cloudy",
            "Cloudy",
            "Light rain",
            "Heavy rain",
            "Snow",
            "Fog",
            "Sunny"
        };
        
        return descriptions[_random.Next(descriptions.Length)];
    }
}