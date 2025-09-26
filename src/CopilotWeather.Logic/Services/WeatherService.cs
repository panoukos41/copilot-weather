using CopilotWeather.Logic.Abstractions;
using CopilotWeather.Logic.Models;

namespace CopilotWeather.Logic.Services;

public class WeatherService : IWeatherService
{
    private readonly IEnumerable<IWeatherProvider> _providers;

    public WeatherService(IEnumerable<IWeatherProvider> providers)
    {
        _providers = providers;
    }

    public async Task<WeatherData> GetWeatherAsync(WeatherRequest request, CancellationToken cancellationToken = default)
    {
        var provider = _providers.FirstOrDefault();
        if (provider == null)
            throw new InvalidOperationException("No weather providers available");

        return await provider.GetWeatherAsync(request, cancellationToken);
    }

    public async Task<IEnumerable<WeatherData>> GetWeatherFromAllProvidersAsync(WeatherRequest request, CancellationToken cancellationToken = default)
    {
        var tasks = _providers.Select(p => p.GetWeatherAsync(request, cancellationToken));
        var results = await Task.WhenAll(tasks);
        return results;
    }

    public async Task<WeatherData> GetAggregatedWeatherAsync(WeatherRequest request, CancellationToken cancellationToken = default)
    {
        var results = await GetWeatherFromAllProvidersAsync(request, cancellationToken);
        var weatherDataList = results.ToList();
        
        if (weatherDataList.Count == 0)
            throw new InvalidOperationException("No weather data available");

        // Simple aggregation - average temperature and humidity
        var avgTemp = weatherDataList.Average(w => w.Temperature);
        var avgHumidity = weatherDataList.Average(w => w.Humidity);
        var firstResult = weatherDataList.First();

        return new WeatherData
        {
            Location = firstResult.Location,
            Temperature = Math.Round(avgTemp, 1),
            Humidity = Math.Round(avgHumidity, 1),
            Description = $"Aggregated from {weatherDataList.Count} providers",
            Provider = "Aggregated",
            Timestamp = DateTime.UtcNow
        };
    }
}