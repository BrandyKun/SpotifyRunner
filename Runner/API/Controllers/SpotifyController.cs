using Application.Interface;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using SpotifyAPI.Web;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SpotifyController : ControllerBase
{
    private readonly ILogger<SpotifyController> _logger;
    private readonly ISpotifyDataService _spotifyService;
    public SpotifyController(ILogger<SpotifyController> logger, ISpotifyDataService spotifyService)
    {
        _spotifyService = spotifyService;
        _logger = logger;
    }

    [HttpGet("user")]
    public async Task<PublicUser> GetUser()
    {
        var user = await _spotifyService.GetUserInfo();
        return user;
    }

    [HttpGet("playlists")]
    public async Task<IEnumerable<FullPlaylist>> GetUserPlaylist(){
        
        var playlists = await _spotifyService.GetAllUsersPlaylist();
        return playlists;
    }
}
