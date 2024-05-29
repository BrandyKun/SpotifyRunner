using Domain.Entities;

namespace Application.Interface;

public interface ISpotifyAuthService
{
    Task<SpotifyToken> GetAccessTokenAsync(string code);
    Task<SpotifyToken> RefreshAccessTokenAsync(string refreshToken);
    Task<SpotifyToken> GetOrRefreshToken(string code= null);
}
