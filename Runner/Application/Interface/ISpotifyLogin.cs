using Domain.Entities;

namespace Application.Interface;

public interface ISpotifyLogin
{
    Task<AuthResult> GetToken( string code);
    Task<string> GetAuthCodeAsync();
    Task<AuthResult> ExchangeCodeForAccessToken( string code, string state);
    Uri BuildUri();
    Task<string> ReturnAccessTokenFromRefreshToken();
}
