using System.Net;
using Application.Interface;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;
    private readonly ISpotifyLogin _spotifyService;
    private readonly IConfiguration _config;

    public WeatherForecastController(ILogger<WeatherForecastController> logger, ISpotifyLogin spotifyService, IConfiguration config)
    {
        _config = config;
        _spotifyService = spotifyService;
        _logger = logger;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
    }

    [HttpPost, Route("Token")]
    public async Task<string> RequestToken()
    {
        var token = await _spotifyService.GetToken(_config["Spotify:client_id"], _config["Spotify:client_secret"]);
        var gotThings = token;
        return "true";
    }
    [HttpGet, Route("authCode")]
    public async Task<string> RequestAuthCode()
    {
        var token = await _spotifyService.GetAuthCodeAsync(_config["Spotify:client_id"], _config["Spotify:client_secret"]);
        var gotThings = token.ToString();
        return token;
    }
}
