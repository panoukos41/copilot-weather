namespace CopilotWeather.Logic.Models;

/// <summary>
/// Represents a request for weather information for a specific location.
/// </summary>
public class WeatherRequest
{
    /// <summary>
    /// The location for which to retrieve weather information.
    /// </summary>
    public required string Location { get; set; }

    /// <summary>
    /// Optional provider name to use for weather data retrieval.
    /// If not specified, the first available provider will be used.
    /// </summary>
    public string? Provider { get; set; }
}