namespace CopilotWeather.Logic.Models;

/// <summary>
/// Represents weather data for a specific location.
/// </summary>
public class WeatherData
{
    /// <summary>
    /// The location for which this weather data applies.
    /// </summary>
    public required string Location { get; set; }

    /// <summary>
    /// The temperature in degrees Celsius.
    /// </summary>
    public double Temperature { get; set; }

    /// <summary>
    /// The humidity percentage (0-100).
    /// </summary>
    public double Humidity { get; set; }

    /// <summary>
    /// A textual description of the weather conditions.
    /// </summary>
    public required string Description { get; set; }

    /// <summary>
    /// The name of the weather provider that supplied this data.
    /// </summary>
    public required string Provider { get; set; }

    /// <summary>
    /// The timestamp when this weather data was retrieved.
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}