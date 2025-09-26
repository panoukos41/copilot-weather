# Copilot Weather

A weather API built with .NET 9 that supports multiple weather providers and can aggregate data from them.

## Architecture

The solution is structured with two main projects:

- **CopilotWeather.Logic** - Core business logic and weather provider abstractions
- **CopilotWeather.Api** - Web API endpoints

## Features

- Multiple weather provider support
- Weather data aggregation from multiple providers
- Clean architecture with dependency injection
- Centralized MSBuild configuration
- .NET 9 with latest language features

## Project Structure

```
├── Directory.Build.props    # MSBuild properties (targets .NET 9, artifacts config)
├── Packages.props          # Centralized package management
├── CopilotWeather.sln      # Solution file
└── src/
    ├── CopilotWeather.Logic/   # Core logic and abstractions
    └── CopilotWeather.Api/     # Web API project
```

## API Endpoints

- `GET /weather/{location}` - Get weather from the first available provider
- `GET /weather/{location}/all-providers` - Get weather from all providers
- `GET /weather/{location}/aggregated` - Get aggregated weather data
- `GET /health` - Health check endpoint

## Building and Running

```bash
# Restore dependencies
dotnet restore

# Build the solution
dotnet build

# Run the API
dotnet run --project src/CopilotWeather.Api

# Test the API
curl http://localhost:5000/weather/London
```

## Configuration

The project uses MSBuild Directory.Build.props for:
- .NET 9 targeting
- Centralized artifacts directory
- Common project properties
- Package version management via Packages.props