using System.Diagnostics;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ILogger<AuthController> _logger;
    private readonly IConfiguration _configuration;
    private readonly SpotifyTokenService _spotifyTokenService;

    public AuthController(ILogger<AuthController> logger, SpotifyTokenService spotifyTokenService, IConfiguration configuration)
    {
        _spotifyTokenService = spotifyTokenService;
        _configuration = configuration;
        _logger = logger;
    }

    [HttpGet("login")]
    public IActionResult Login()
    {
        var clientId = _configuration["Spotify:ClientId"];
        var redirectUri = _configuration["Spotify:RedirectUri"];
        var scopes = "user-read-private playlist-modify-public playlist-modify-private";
        var authUrl = $"https://accounts.spotify.com/authorize?response_type=code&client_id={clientId}&redirect_uri={redirectUri}&scope={Uri.EscapeDataString(scopes)}";

        return Ok(authUrl);
    }

    [HttpGet("callback")]
    public async Task<IActionResult> CallBack(string code)
    {
        var token = await _spotifyTokenService.GetOrRefreshToken(code);
        return Ok(token);
    }

}
