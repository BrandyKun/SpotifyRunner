using System.Net;
using Application.Interface;
using Domain.Entities;

namespace Infrastructure.Services;

public class SpotifydataService : ISpotifyDataService
{
    private readonly SpotifyTokenService _tokenService;
    private readonly HttpClient _httpClient;
    private string spootifyBaseUrl = "https://api.spotify.com/v1/";
    // private ClientDetail spotifyDetails;
    private SpotifyTokentoReturn token = new SpotifyTokentoReturn();
    public SpotifydataService(SpotifyTokenService tokenService, HttpClient httpClient)
    {
        _tokenService = tokenService;
        _httpClient = httpClient;
    }
    // public ClientDetail Details
    // {
    //     get{
    //         ClientDetail cd = new ClientDetail();
    //         if (spotifyDetails != null)
    //         {}cd 
    //     }
    // }

    public string AccessToken
    {
        get
        {
            if (token.IsValid || _tokenService.ReturnAccessTokenFromRefreshToken().Result)
            { return token.AccessToken; }
            return _tokenService.AccessToken;
        }
    }
    public Task<SpotifyUser> GetUserInfo()
    {
        if(string.IsNullOrEmpty(AccessToken))
        {
            throw new ArgumentNullException("No access token available");
        }

        throw new NotImplementedException();
    }
}
