namespace CopilotWeather.Logic.Models;

public record WeatherData
{
    public string Location { get; init; } = string.Empty;
    public double Temperature { get; init; }
    public double Humidity { get; init; }
    public string Description { get; init; } = string.Empty;
    public string Provider { get; init; } = string.Empty;
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
}

public record WeatherRequest
{
    public string Location { get; init; } = string.Empty;
    public string? Units { get; init; } = "metric";
}
