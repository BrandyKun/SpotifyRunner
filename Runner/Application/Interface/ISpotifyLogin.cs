using Domain.Entities;

namespace Application.Interface;

public interface ISpotifyLogin
{
    Task<AuthResult> GetToken(string clientId, string clientSecret, string code);
    Task<string> GetAuthCodeAsync(string clientId, string clientSecret);
    Task<AuthResult> ExchangeCodeForAccessToken( string code, string state, string clientId, string clientSecret);
    Uri BuildUri(string clientId);
    Task<string> ReturnAccessTokenFromRefreshToken();
}
