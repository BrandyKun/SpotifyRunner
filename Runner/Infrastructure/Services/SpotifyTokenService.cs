using Application.Interface;
using Domain.Entities;

namespace Infrastructure.Services;

public class SpotifyTokenService
{
    private readonly ISpotifyAuthService _spotifyAuthService;
    private SpotifyToken _currenttoken;

    public SpotifyTokenService(ISpotifyAuthService spotifyAuthService)
    {
        _spotifyAuthService = spotifyAuthService;
    }
}
