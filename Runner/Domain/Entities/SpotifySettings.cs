namespace Domain.Entities;

public class SpotifySettings
{
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    public string RedirectUri { get; set; }
}


//set the cleint secrets environments and all and add to startup