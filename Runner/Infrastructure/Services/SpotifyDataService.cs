using System.Net.Http.Headers;
using System.Text.Json.Serialization;
using Application.Interface;
using Domain.Common;
using Domain.Entities.UserEntities;
using Newtonsoft.Json;
using SpotifyAPI.Web;

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

    public async Task<PublicUser> GetUserInfo()
    {
       var privateUserInfo = await GetUserProfile().ConfigureAwait(false);

       if(privateUserInfo == null)
       throw new ArgumentNullException("we could not retrieve your profile details");

       var token = await _spotifyAuthService.GetOrRefreshToken();
        if (token == null)
        throw new ArgumentNullException(nameof(token));

        var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"{Constants.userProfile}/{privateUserInfo.Id}" );
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
        requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        
        var httpClient = _httpClientFactory.CreateClient();

        var response = await httpClient.SendAsync(requestMessage);

        response.EnsureSuccessStatusCode();

        var data = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        PublicUser user = JsonConvert.DeserializeObject<PublicUser>(data);

        return user;
    }

    public async Task<PrivateUser> GetUserProfile()
    {
        var token = await _spotifyAuthService.GetOrRefreshToken();
        if (token == null)
        throw new ArgumentNullException(nameof(token));

        var requestMessage = new HttpRequestMessage(HttpMethod.Get, Constants.currentUser);
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
        requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        
        var httpClient = _httpClientFactory.CreateClient();

        var response = await httpClient.SendAsync(requestMessage);

        response.EnsureSuccessStatusCode();

        var data = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        PrivateUser user = JsonConvert.DeserializeObject<PrivateUser>(data);

        return user;
    }
}
