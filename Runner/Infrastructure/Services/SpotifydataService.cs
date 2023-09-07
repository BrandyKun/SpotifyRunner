using Application.Interface;
using Domain.Entities;

namespace Infrastructure.Services;

public class SpotifydataService : ISpotifyDataService
{
    private readonly SpotifyTokenService _tokenService;
    private ClientDetail spotifyDetails;
    private SpotifyTokentoReturn token = new SpotifyTokentoReturn();
    public SpotifydataService(SpotifyTokenService tokenService)
    {
        _tokenService = tokenService;
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
            
            ///call endpoint to get user info
            
        }
    }
}
