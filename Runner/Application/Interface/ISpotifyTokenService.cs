namespace Application.Interface;

public interface ISpotifyTokenService
{
    Task<string> GetSpotifyInformation();
}
