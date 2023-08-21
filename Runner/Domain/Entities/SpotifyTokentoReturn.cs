namespace Domain.Entities;

public class SpotifyTokentoReturn : SpotifyToken
{
    public bool HasExpired { get => !string.IsNullOrEmpty(AccessToken) && Expirytime > DateTime.UtcNow; }
}
