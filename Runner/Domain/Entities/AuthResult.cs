namespace Domain.Entities;

public class AuthResult
{
    public string access_token { get; set; } = default(string);
    public string token_type { get; set; }
    public int expires_in { get; set; }
    public string refresh_token { get; set; }
}
