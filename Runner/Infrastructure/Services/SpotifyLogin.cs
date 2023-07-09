using Application.Interface;
using Domain.Entities;
using IdentityModel.Client;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Infrastructure.Services;

public class SpotifyLogin : ISpotifyLogin
{
    private readonly HttpClient _httpClient;
    private readonly string scopes = "user-read-playback-state user-modify-playback-state user-read-currently-playing user-read-playback-position user-top-read user-read-recently-played playlist-read-private playlist-read-collaborative playlist-modify-private playlist-modify-public";

    private readonly string authCodeEndpoint = "https://accounts.spotify.com/authorize";
    private readonly string tokenEndpoint = "https://accounts.spotify.com/api/token";

    public SpotifyLogin(HttpClient httpClient)
    {
        _httpClient = httpClient;

    }

    public async Task<string> GetAuthCodeAsync(string clientId, string clientSecret)
    {

        string urlParams = $"response_type=code&state=16&client_id={clientId}&redirect_uri=http://localhost:5039&scope={scopes}&show_dialog=true";
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"{authCodeEndpoint}?{urlParams}");

        HttpRequest
        var messageResponse = await _httpClient.RequestAuthorizationCodeTokenAsync(new AuthorizationCodeTokenRequest()
        {
            Address = tokenEndpoint,
            ClientId = clientId,
            ClientSecret = clientSecret,
            Code = "16",
            RedirectUri = "http://localhost:5039"
            // Scope = scopes,
            
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

    public async Task<string> GetToken(string clientId, string clientSecret)
    {
        string token = "";
        try
        {
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, tokenEndpoint);

            requestMessage.Headers.Authorization = new AuthenticationHeaderValue(
                "Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}")));

            requestMessage.Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                {"grant_type", "client_credentials"}
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



}
