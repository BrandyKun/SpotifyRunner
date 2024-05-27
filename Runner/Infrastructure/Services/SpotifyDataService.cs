using Application.Interface;
using Domain.Entities;

namespace Infrastructure.Services;

public class SpotifyDataService : ISpotifyDataService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ISpotifyAuthService _spotifyAuthService;
    public SpotifyDataService(IHttpClientFactory httpClientFactory, ISpotifyAuthService spotifyAuthService)
    {
        _spotifyAuthService = spotifyAuthService;
        _httpClientFactory = httpClientFactory;
    }
    public async Task<SpotifyUser> GetUserInfo()
    {
        throw new NotImplementedException();
    }
}
