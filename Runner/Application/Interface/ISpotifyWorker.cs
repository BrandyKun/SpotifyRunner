namespace Application.Interface;

public interface ISpotifyWorker
{
    Task<string> GetSpotifyInformation();
}
