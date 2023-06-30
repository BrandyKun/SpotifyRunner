using Application.Interface;

namespace Infrastructure.Services;

public class SpotifyLogin : ISpotifyLogin
{
    Task<string> ISpotifyLogin.getToken(string clientId, string clientSecret)
    {
        throw new NotImplementedException();
    }
}
