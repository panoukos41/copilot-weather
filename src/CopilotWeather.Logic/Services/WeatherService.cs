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

    public async Task<Result<WeatherData>> GetWeatherAsync(WeatherRequest request, CancellationToken cancellationToken = default)
    {
        var provider = string.IsNullOrEmpty(request.Provider) 
            ? _providers.FirstOrDefault()
            : _providers.FirstOrDefault(p => p.Name.Equals(request.Provider, StringComparison.OrdinalIgnoreCase));
            
        if (provider == null)
        {
            if (string.IsNullOrEmpty(request.Provider))
            {
                return Result<WeatherData>.Failure("No weather providers available");
            }
            
            var availableProviders = string.Join(", ", _providers.Select(p => p.Name));
            return Result<WeatherData>.Failure($"Weather provider '{request.Provider}' not found. Available providers: {availableProviders}");
        }

        var providerRequest = new ProviderWeatherRequest { Location = request.Location };
        var weatherData = await provider.GetWeatherAsync(providerRequest, cancellationToken);
        return Result<WeatherData>.Success(weatherData);
    }

    public async Task<Dictionary<string, WeatherData>> GetWeatherFromAllProvidersAsync(string location, CancellationToken cancellationToken = default)
    {
        var providerRequest = new ProviderWeatherRequest { Location = location };
        var tasks = _providers.Select(async p => new { Provider = p, Data = await p.GetWeatherAsync(providerRequest, cancellationToken) });
        var results = await Task.WhenAll(tasks);
        return results.ToDictionary(r => r.Provider.Name, r => r.Data);
    }

    public async Task<WeatherData> GetAggregatedWeatherAsync(string location, CancellationToken cancellationToken = default)
    {
        var results = await GetWeatherFromAllProvidersAsync(location, cancellationToken);
        var weatherDataList = results.Values.ToList();
        
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

    public IEnumerable<string> GetAvailableProviders()
    {
        return _providers.Select(p => p.Name);
    }
}