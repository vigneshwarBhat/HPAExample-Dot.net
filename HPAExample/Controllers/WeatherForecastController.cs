using HPAExample;
using HPAExample.Model;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Metrics;

namespace API1.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;
    private readonly Counter<long> _freezingDaysCounter;

    public WeatherForecastController(ILogger<WeatherForecastController> logger, Instrumentation instrumentation)
    {
        _logger = logger;
        _freezingDaysCounter = instrumentation.FreezingDaysCounter;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        var rng = new Random();
        var forecast = Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateTime.Now.AddDays(index),
            TemperatureC = rng.Next(-20, 55),
            Summary = Summaries[rng.Next(Summaries.Length)],
        })
        .ToArray();

        // Optional: Count the freezing days
        _freezingDaysCounter.Add(forecast.Count(f => f.TemperatureC < 0));

        _logger.LogInformation(
            "WeatherForecasts generated {count}: {forecasts}",
            forecast.Length,
            forecast);

        return forecast;
    }
}