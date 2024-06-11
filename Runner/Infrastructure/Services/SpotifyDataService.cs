using System.Net.Http.Headers;
using System.Text.Json.Serialization;
using System.Xml.Schema;
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

        if (privateUserInfo == null)
            throw new ArgumentNullException("we could not retrieve your profile details");

        var token = await _spotifyAuthService.GetOrRefreshToken();
        if (token == null)
            throw new ArgumentNullException(nameof(token));

        var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"{Constants.userProfile}/{privateUserInfo.Id}");
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

        var requestMessage = new HttpRequestMessage(HttpMethod.Get, Constants.baseUrl);
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
        requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var httpClient = _httpClientFactory.CreateClient();

        var response = await httpClient.SendAsync(requestMessage);

        response.EnsureSuccessStatusCode();

        var data = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        PrivateUser user = JsonConvert.DeserializeObject<PrivateUser>(data);

        return user;
    }

    public async Task<IEnumerable<FullPlaylist>> GetAllUsersPlaylist()
    {
        var allPlaylists = new List<FullPlaylist>();
        var token = await _spotifyAuthService.GetOrRefreshToken();
        if (token == null)
            throw new ArgumentNullException(nameof(token));

        string nextUrl = Constants.baseUrl + "/playlists";
        var httpClient = _httpClientFactory.CreateClient();

        do
        {

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, nextUrl);

            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
            requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


            var response = await httpClient.SendAsync(requestMessage);

            var data = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var playlistPage = JsonConvert.DeserializeObject<Paging<FullPlaylist>>(data);

            if (playlistPage?.Items != null)
            {
                allPlaylists.AddRange(playlistPage.Items);
            }
            nextUrl = playlistPage?.Next;
        }
        while (!string.IsNullOrEmpty(nextUrl));

        return allPlaylists;
    }
}
