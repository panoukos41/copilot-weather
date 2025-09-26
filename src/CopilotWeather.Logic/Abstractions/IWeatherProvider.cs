using CopilotWeather.Logic.Models;

namespace CopilotWeather.Logic.Abstractions;

public interface IWeatherProvider
{
    string Name { get; }
    Task<WeatherData> GetWeatherAsync(ProviderWeatherRequest request, CancellationToken cancellationToken = default);
    Task<bool> IsAvailableAsync(CancellationToken cancellationToken = default);
}

public interface IWeatherService
{
    Task<Result<WeatherData>> GetWeatherAsync(WeatherRequest request, CancellationToken cancellationToken = default);
    Task<Dictionary<string, WeatherData>> GetWeatherFromAllProvidersAsync(string location, CancellationToken cancellationToken = default);
    Task<WeatherData> GetAggregatedWeatherAsync(string location, CancellationToken cancellationToken = default);
    IEnumerable<string> GetAvailableProviders();
}