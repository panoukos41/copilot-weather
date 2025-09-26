namespace CopilotWeather.Logic.Models;

/// <summary>
/// Represents a weather request specifically for weather providers.
/// This model only contains the essential data needed by providers.
/// </summary>
public class ProviderWeatherRequest
{
    /// <summary>
    /// The location for which to retrieve weather information.
    /// </summary>
    public required string Location { get; set; }
}