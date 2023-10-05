using Domain.Entities;

namespace Application.Interface;

public interface ISpotifyLogin
{
    Task<string> GetAuthCodeAsync();
    Task<AuthResult> ExchangeCodeForAccessToken( string code, string state);
    Uri BuildUri();
}
