using Domain.Entities;

namespace Application.Interface;

public interface ISpotifyLogin
{
    Task<string> GetToken(string clientId, string clientSecret);
    // Task<AuthCode> GetAuthCodeAsync(string clientId, string clientSecret);
    Task<string> GetAuthCodeAsync(string clientId, string clientSecret);
}
