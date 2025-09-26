namespace CopilotWeather.Logic.Configuration;

/// <summary>
/// Configuration options for the AccuWeather API provider.
/// </summary>
public class AccuWeatherOptions
{
    public const string SectionName = "AccuWeather";

    /// <summary>
    /// The API key for AccuWeather API.
    /// </summary>
    public required string ApiKey { get; set; }

    /// <summary>
    /// The base URL for the AccuWeather API.
    /// </summary>
    public string BaseUrl { get; set; } = "http://dataservice.accuweather.com";

    /// <summary>
    /// The timeout in seconds for HTTP requests.
    /// </summary>
    public int TimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// Whether to use the metric system (Celsius) or imperial (Fahrenheit).
    /// </summary>
    public bool UseMetric { get; set; } = true;

    /// <summary>
    /// The API version to use.
    /// </summary>
    public string ApiVersion { get; set; } = "v1";
}