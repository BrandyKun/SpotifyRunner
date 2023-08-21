namespace Domain.Entities;

public class SpotifyToken
{
    public int Id { get; set; }
    public string? AccessToken { get; set; } = default;
    public DateTime Expirytime { get; set; }
    public string RefreshToken { get; set; }
}
