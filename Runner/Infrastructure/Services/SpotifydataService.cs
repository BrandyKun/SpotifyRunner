using System.Net;
using System.Net.Http.Headers;
using Application.Interface;
using Domain.Entities;

namespace Infrastructure.Services;

public class SpotifyDataService : ISpotifyDataService
{
    private readonly ISpotifyTokenService _tokenService;
    private readonly HttpClient _httpClient;
    private string spotifyBaseUrl = "https://api.spotify.com/v1/";
    // private ClientDetail spotifyDetails;
    private SpotifyTokentoReturn token = new SpotifyTokentoReturn();
    public SpotifyDataService( HttpClient httpClient)
    {
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

    // public string AccessToken
    // {
    //     get
    //     {
    //         if (token.IsValid || _tokenService.ReturnAccessTokenFromRefreshToken().Result)
    //         { return token.AccessToken; }
    //         return _tokenService.
    //     }
    // }

    //get token from db
    // check if db token is valid, if nto refresh and get a new one
    // assign it to value. 

    
    public async Task<SpotifyUser> GetUserInfo()
    {
        // if(string.IsNullOrEmpty(AccessToken))
        // {
        //     throw new ArgumentException();
        // }

        // HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"{spotifyBaseUrl}/me");
        // request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);

        // var response = await _httpClient.SendAsync(request, CancellationToken.None).ConfigureAwait(false);


        HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, $"{spotifyBaseUrl}/me");

        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);


        throw new NotImplementedException();
    }
}
