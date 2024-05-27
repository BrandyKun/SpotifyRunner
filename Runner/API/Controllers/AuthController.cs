using System.Diagnostics;
using Application.Interface;
using Domain.Entities;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ILogger<AuthController> _logger;
    private readonly IConfiguration _configuration;
    private readonly ISpotifyAuthService _spotifyAuthService;
    private SpotifySettings _spotifySettings ;

    public AuthController(ILogger<AuthController> logger, ISpotifyAuthService spotifyAuthService, IOptions<SpotifySettings> spotifySettings)
    {
        _spotifySettings = spotifySettings.Value;
        _spotifyAuthService = spotifyAuthService;
        _logger = logger;
    }

    [HttpGet("login")]
    public IActionResult Login()
    {
        var clientId = _spotifySettings.ClientId;
        var redirectUri = _spotifySettings.RedirectUri;
        var scopes = "user-read-private playlist-modify-public playlist-modify-private";
        var authUrl = $"https://accounts.spotify.com/authorize?response_type=code&client_id={clientId}&redirect_uri={redirectUri}&scope={Uri.EscapeDataString(scopes)}";

        return Ok(authUrl);
    }

    [HttpGet("callback")]
    public async Task<IActionResult> CallBack(string code)
    {
        var token = await _spotifyAuthService.GetOrRefreshToken(code);
        return Ok(token);
    }

}
