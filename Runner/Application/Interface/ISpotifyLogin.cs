namespace Application.Interface;

public interface ISpotifyLogin
{
    Task<string> GetToken(string clientId, string clientSecret);
}
