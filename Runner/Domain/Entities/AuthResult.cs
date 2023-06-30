namespace Domain.Entities;

public class AuthResult
{
    public string AccessToken { get; set; }
    public string TokenType { get; set; }
    public int Duration { get; set; }
}
