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

    public async Task<SpotifyToken> GetOrRefreshToken(string code = null)
    {
        if (_currenttoken == null || _currenttoken.ExpiryTime <= DateTime.UtcNow)
        {
            if (_currenttoken == null && !String.IsNullOrEmpty(code))
            {
                _currenttoken = await _spotifyAuthService.GetAccessTokenAsync(code);

            }
            else if (_currenttoken != null)
            {
                _currenttoken = await _spotifyAuthService.RefreshAccessTokenAsync(_currenttoken.RefreshToken);
            }
            else
            {
                throw new InvalidOperationException("No valid code or token available.");
            }
        }

        return _currenttoken;
    }
}
