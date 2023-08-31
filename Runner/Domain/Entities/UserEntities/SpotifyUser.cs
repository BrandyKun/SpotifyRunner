namespace Domain.Entities;

public class SpotifyUser
{
    public int Id { get; set; }
    public string? Country { get; set; } = default;
    public string? DisplayName { get; set; } = default;
    public string? Email { get; set; } =default;
    public Follower? follower { get; set; }
    public string? Url { get; set; }
    public SpotifyImage? Image { get; set; }
    public string? Type { get; set; }
    public string? Product { get; set; }
    public string? URI { get; set; }
}
