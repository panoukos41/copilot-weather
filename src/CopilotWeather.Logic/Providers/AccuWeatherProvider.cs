using System.Text.Json;
using CopilotWeather.Logic.Abstractions;
using CopilotWeather.Logic.Configuration;
using CopilotWeather.Logic.Models;

namespace CopilotWeather.Logic.Providers;

public class AccuWeatherProvider : IWeatherProvider
{
    private readonly HttpClient _httpClient;
    private readonly AccuWeatherOptions _options;
    private readonly JsonSerializerOptions _jsonOptions;

    public AccuWeatherProvider(HttpClient httpClient, AccuWeatherOptions options)
    {
        _httpClient = httpClient;
        _options = options;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };

        // Configure HttpClient timeout
        _httpClient.Timeout = TimeSpan.FromSeconds(_options.TimeoutSeconds);
    }

    public string Name => "AccuWeather";

    public async Task<WeatherData> GetWeatherAsync(ProviderWeatherRequest request, CancellationToken cancellationToken = default)
    {
        // First, get the location key for the given location
        var locationKey = await GetLocationKeyAsync(request.Location, cancellationToken);
        
        // Then get the current weather conditions
        var currentConditionsUrl = $"{_options.BaseUrl}/currentconditions/{_options.ApiVersion}/{locationKey}";
        var queryParams = $"?apikey={_options.ApiKey}&details=true";
        
        var response = await _httpClient.GetAsync($"{currentConditionsUrl}{queryParams}", cancellationToken);
        response.EnsureSuccessStatusCode();
        
        var jsonContent = await response.Content.ReadAsStringAsync(cancellationToken);
        var weatherConditions = JsonSerializer.Deserialize<AccuWeatherCondition[]>(jsonContent, _jsonOptions);
        
        if (weatherConditions == null || !weatherConditions.Any())
        {
            throw new InvalidOperationException($"No weather data found for location: {request.Location}");
        }
        
        var condition = weatherConditions.First();
        
        return new WeatherData
        {
            Location = request.Location,
            Temperature = _options.UseMetric 
                ? condition.Temperature.Metric.Value 
                : ConvertFahrenheitToCelsius(condition.Temperature.Imperial.Value),
            Humidity = condition.RelativeHumidity ?? 0,
            Description = condition.WeatherText ?? "Unknown",
            Provider = Name,
            Timestamp = DateTime.UtcNow
        };
    }

    public async Task<bool> IsAvailableAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrEmpty(_options.ApiKey))
                return false;
            
            // Simple health check - try to get a well-known location
            var testUrl = $"{_options.BaseUrl}/locations/{_options.ApiVersion}/cities/search";
            var queryParams = $"?apikey={_options.ApiKey}&q=London";
            
            var response = await _httpClient.GetAsync($"{testUrl}{queryParams}", cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    private async Task<string> GetLocationKeyAsync(string location, CancellationToken cancellationToken)
    {
        var locationSearchUrl = $"{_options.BaseUrl}/locations/{_options.ApiVersion}/cities/search";
        var queryParams = $"?apikey={_options.ApiKey}&q={Uri.EscapeDataString(location)}";
        
        var response = await _httpClient.GetAsync($"{locationSearchUrl}{queryParams}", cancellationToken);
        response.EnsureSuccessStatusCode();
        
        var jsonContent = await response.Content.ReadAsStringAsync(cancellationToken);
        var locations = JsonSerializer.Deserialize<AccuWeatherLocation[]>(jsonContent, _jsonOptions);
        
        if (locations == null || !locations.Any())
        {
            throw new InvalidOperationException($"Location not found: {location}");
        }
        
        return locations.First().Key;
    }
    
    private static double ConvertFahrenheitToCelsius(double fahrenheit)
    {
        return Math.Round((fahrenheit - 32) * 5.0 / 9.0, 1);
    }
}

// Data Transfer Objects for AccuWeather API responses
internal class AccuWeatherLocation
{
    public required string Key { get; set; }
    public required string LocalizedName { get; set; }
}

internal class AccuWeatherCondition
{
    public required string WeatherText { get; set; }
    public required AccuWeatherTemperature Temperature { get; set; }
    public int? RelativeHumidity { get; set; }
}

internal class AccuWeatherTemperature
{
    public required AccuWeatherTemperatureValue Metric { get; set; }
    public required AccuWeatherTemperatureValue Imperial { get; set; }
}

internal class AccuWeatherTemperatureValue
{
    public double Value { get; set; }
    public required string Unit { get; set; }
}