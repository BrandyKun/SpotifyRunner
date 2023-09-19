using Application.Interface;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SpotifyController : ControllerBase
{
    private readonly ISpotifyDataService _spotifyDataService;
    public SpotifyController(ISpotifyDataService spotifyDataService)
    {
        _spotifyDataService = spotifyDataService;
    }

    [HttpGet("user")]
    public async Task<string> Getdata()
    {
        var something = await _spotifyDataService.GetUserInfo();
        return "hello";
    }
}
