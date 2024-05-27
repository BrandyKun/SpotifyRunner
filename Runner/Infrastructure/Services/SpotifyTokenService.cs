using Application.Interface;
using Domain.Entities;

namespace Infrastructure.Services;

public class SpotifyTokenService
{
    private readonly ISpotifyAuthService _spotifyAuthService;
    private SpotifyToken _currenttoken;
    private readonly SpotifyDbContext _context;

    public SpotifyTokenService(ISpotifyAuthService spotifyAuthService, SpotifyDbContext context)
    {
        _context = context;
        _spotifyAuthService = spotifyAuthService;
    }

    public async Task<SpotifyToken> GetOrRefreshToken(string code = null)
    {

        _currenttoken = _context.SpotifyTokens.FirstOrDefault() ?? new SpotifyToken();
        
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
