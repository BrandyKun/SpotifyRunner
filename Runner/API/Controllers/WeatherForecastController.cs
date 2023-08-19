using System.Threading.Tasks;
using System.Net;
using Application.Interface;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

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
    public IOptions<AppSettings> _appSettings { get; }

    public WeatherForecastController(ILogger<WeatherForecastController> logger, ISpotifyLogin spotifyService, IConfiguration config, IOptions<AppSettings> appSettings)
    {
        _appSettings = appSettings;
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
        var token = await _spotifyService.GetToken(_appSettings.Value.ClientId, _appSettings.Value.ClientSecret, "something");
        var gotThings = token;
        return "true";
    }
    [HttpGet, Route("authCode")]
    public Task<string> RequestAuthCode()
    {
        return Task.Run(() =>
        {
            var address = _spotifyService.BuildUri(_appSettings.Value.ClientId);
            return address.ToString();
        });
    }
}
