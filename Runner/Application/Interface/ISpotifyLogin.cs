namespace Application.Interface;

public interface ISpotifyLogin
{
    Task<string> getToken(string clientId, string clientSecret);
}
