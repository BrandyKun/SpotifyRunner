using System.Security.Principal;
using Application.Interface;
using Domain.Entities;
using IdentityModel.Client;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using AutoMapper;

namespace Infrastructure.Services;

public class SpotifyLogin : ISpotifyLogin
{
    private readonly HttpClient _httpClient;
    private readonly SpotifyDbContext _spotifyDbContext;
    private readonly string scopes = "user-read-playback-state user-modify-playback-state user-read-currently-playing user-read-playback-position user-top-read user-read-recently-played playlist-read-private playlist-read-collaborative playlist-modify-private playlist-modify-public";

    private readonly string authCodeEndpoint = "https://accounts.spotify.com/authorize?";
    private readonly string tokenEndpoint = "https://accounts.spotify.com/api/token";
    const string redirectSign = "/auth/authcode";
    private readonly IMapper _mapper;

    public SpotifyLogin(HttpClient httpClient, SpotifyDbContext spotifyDbContext, IMapper mapper)
    {
        _mapper = mapper;
        _httpClient = httpClient;
        _spotifyDbContext = spotifyDbContext;
    }

    public async Task<ClientDetail> GetClientDetailsAsync()
    {
        var clientDets = await _spotifyDbContext.ClientDetails.FirstOrDefaultAsync();

        if (clientDets == null)
            throw new ArgumentNullException();

        return clientDets;
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

    public async Task<AuthResult> GetToken(string clientId, string clientSecret, string code)
    {
        AuthResult token = new AuthResult();

        try
        {
            if (!string.IsNullOrEmpty(code))
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

                token = await JsonSerializer.DeserializeAsync<AuthResult>(responseData);

                DateTime time = DateTime.Now;
                time = time.AddSeconds(token.expires_in);
                token.ExpiryTime = time;
                var newSave = _mapper.Map<SpotifyToken>(token);
                _spotifyDbContext.SpotifyTokens.Add(newSave);
                _spotifyDbContext.SaveChanges();

            }
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

        StringBuilder sb = new StringBuilder();
        sb.Append("/authorize?");
        sb.Append("response_type=code");
        sb.Append("&state=16");
        sb.Append($"&client_id={clientId}");
        sb.Append($"&redirect_uri={Uri.EscapeUriString("http://localhost:3000/callback")}");
        // sb.Append($"&redirect_uri={Uri.EscapeUriString("http://localhost:5039/callback")}");
        sb.Append($"&scope={scopes}");
        // sb.Append("&show_dialog=true");
        return new Uri(new Uri(authCodeEndpoint), sb.ToString());
    }

    public async Task<AuthResult> ExchangeCodeForAccessToken(string code, string state, string clientId, string clientSecret)
    {
        var token = await GetToken(clientId, clientSecret, code);

        return token;
    }

    public async Task<string> ReturnAccessTokenFromRefreshToken()
    {
        try
        {
            var token = await _spotifyDbContext.SpotifyTokens.OrderByDescending(t => t.Expirytime)
                                                                .FirstOrDefaultAsync();
            if (token == null)
                throw new ArgumentNullException("token");


            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, tokenEndpoint);

            requestMessage.Headers.Authorization = new AuthenticationHeaderValue(
                "Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes("{clientId}")));

            requestMessage.Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                {"grant_type", "refresh_token"},
                {"refresh_token", $"{token.RefreshToken}"}

            });

            var response = await _httpClient.SendAsync(requestMessage);

            response.EnsureSuccessStatusCode();
            var responseData = await response.Content.ReadAsStreamAsync();

            var convertedResult = await JsonSerializer.DeserializeAsync<AuthResult>(responseData);

            if (convertedResult == null)
                throw new HttpRequestException();

            var newToken = new SpotifyToken
            {
                AccessToken = convertedResult?.access_token,
                RefreshToken = string.IsNullOrEmpty(convertedResult?.refresh_token) ? convertedResult.refresh_token : token.RefreshToken,
                Expirytime = DateTimeOffset.FromUnixTimeSeconds((long)convertedResult.expires_in).DateTime
            };

            _spotifyDbContext.SpotifyTokens.Add(newToken);
            _spotifyDbContext.SaveChanges();

            return newToken.AccessToken;

        }
        catch (HttpRequestException ex)
        {
            throw new HttpRequestException(ex.Message, ex);
        }
    }
}
