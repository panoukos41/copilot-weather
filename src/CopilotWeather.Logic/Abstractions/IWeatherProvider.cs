using CopilotWeather.Logic.Models;

namespace CopilotWeather.Logic.Abstractions;

public interface IWeatherProvider
{
    string Name { get; }
    Task<WeatherData> GetWeatherAsync(WeatherRequest request, CancellationToken cancellationToken = default);
    Task<bool> IsAvailableAsync(CancellationToken cancellationToken = default);
}

public interface IWeatherService
{
    Task<WeatherData> GetWeatherAsync(WeatherRequest request, CancellationToken cancellationToken = default);
    Task<IEnumerable<WeatherData>> GetWeatherFromAllProvidersAsync(WeatherRequest request, CancellationToken cancellationToken = default);
    Task<WeatherData> GetAggregatedWeatherAsync(WeatherRequest request, CancellationToken cancellationToken = default);
}