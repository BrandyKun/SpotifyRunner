using Application.Interface;
using Domain.Entities;
using IdentityModel.Client;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Infrastructure.Services;

public class SpotifyLogin : ISpotifyLogin
{
    private readonly HttpClient _httpClient;
    private readonly string scopes = "user-read-playback-state user-modify-playback-state user-read-currently-playing user-read-playback-position user-top-read user-read-recently-played playlist-read-private playlist-read-collaborative playlist-modify-private playlist-modify-public";

    private readonly string authCodeEndpoint = "https://accounts.spotify.com/authorize?";
    private readonly string tokenEndpoint = "https://accounts.spotify.com/api/token";
    const string redirectSign = "/auth/authcode";

    public SpotifyLogin(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> GetAuthCodeAsync(string clientId, string clientSecret)
    {

        string urlParams = $"response_type=code&state=16&client_id={clientId}&redirect_uri=http://localhost:5039&scope={scopes}&show_dialog=true";
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"{authCodeEndpoint}?{urlParams}");

        var messageResponse = await _httpClient.RequestAuthorizationCodeTokenAsync(new AuthorizationCodeTokenRequest()
        {
            Address = tokenEndpoint,
            ClientId = clientId,
            ClientSecret = clientSecret,
            Code = "16",
            RedirectUri = "http://localhost:5039"
        });


        AuthResult token = new AuthResult()
        {
            access_token = messageResponse.AccessToken,
            token_type = messageResponse.TokenType,
            expires_in = messageResponse.ExpiresIn,
        };

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        var responseData = await response.Content.ReadAsStreamAsync();

        var authCode = await JsonSerializer.DeserializeAsync<string>(responseData);
        // throw new NotImplementedException();

        return authCode;
    }

    public async Task<string> GetToken(string clientId, string clientSecret, string code)
    {
        string token = "";
        try
        {
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, tokenEndpoint);

            requestMessage.Headers.Authorization = new AuthenticationHeaderValue(
                "Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}")));

            requestMessage.Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                {"grant_type", "authorization_code"},
                {"code", code},
                {"redirect_uri", "http://localhost:3000/callback"}

            });

            var response = await _httpClient.SendAsync(requestMessage);

            response.EnsureSuccessStatusCode();
            var responseData = await response.Content.ReadAsStreamAsync();

            var authResult = await JsonSerializer.DeserializeAsync<AuthResult>(responseData);

            token = authResult?.access_token;
        }
        catch (HttpRequestException ex)
        {
            throw new HttpRequestException(ex.Message, ex);
        }

        return token;
    }


    public Uri BuildUri(string clientId)
    {
        string urlParams = $"response_type=code&state=16&client_id={clientId}&redirect_uri=http://localhost:5039&scope={scopes}&show_dialog=true";

        // UriBuilder builder = new UriBuilder(uriPath);
        StringBuilder sb = new StringBuilder();
        sb.Append("/authorize?");
        sb.Append("response_type=code");
        sb.Append("&state=16");
        sb.Append($"&client_id={clientId}");
        sb.Append($"&redirect_uri={Uri.EscapeUriString("http://localhost:3000/callback")}");
        sb.Append($"&scope={scopes}");
        // sb.Append("&show_dialog=true");
        return new Uri(new Uri(authCodeEndpoint), sb.ToString());
    }

    public async Task<string> ExchangeCodeForAccessToken(string code, string state, string clientId, string clientSecret)
    {
        var token = await GetToken(clientId, clientSecret, code);

        return token;
    }


}
