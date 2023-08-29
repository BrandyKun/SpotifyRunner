namespace Domain.Entities;

public class SpotifyTokentoReturn : SpotifyToken
{
    public bool IsValid { get => !string.IsNullOrEmpty(AccessToken) && Expirytime > DateTime.UtcNow; }
}
