using Application.Interface;
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Domain.Common;

namespace Infrastructure.Services;

public class SpotifyAuthService : ISpotifyAuthService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly TokenService _tokenService;
    private readonly SpotifySettings _spotifySettings;

    public SpotifyAuthService(IHttpClientFactory httpClientFactory, IOptions<SpotifySettings> spotifySettings, TokenService tokenService)
    {
        _httpClientFactory = httpClientFactory;
        _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        _spotifySettings = spotifySettings.Value;
    }
    public async Task<SpotifyToken> GetAccessTokenAsync(string code)
    {
        var clientId = _spotifySettings.ClientId;
        var clientSecret = _spotifySettings.ClientSecret;
        var redirectUri = _spotifySettings.RedirectUri;

        if (!string.IsNullOrEmpty(code) && !string.IsNullOrEmpty(redirectUri) && !string.IsNullOrEmpty(clientId) && !string.IsNullOrEmpty(clientSecret))
        {

            var tokenRequest = new HttpRequestMessage(HttpMethod.Post, Constants.spotifyUrl);
            tokenRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            tokenRequest.Content = new FormUrlEncodedContent(new[]
            {
            new KeyValuePair<string, string>("grant_type", "authorization_code"),
            new KeyValuePair<string, string>("code", code),
            new KeyValuePair<string, string>("redirect_uri", redirectUri),
            new KeyValuePair<string, string>("client_id", clientId),
            new KeyValuePair<string, string>("client_secret", clientSecret)
        });

            var httpClient = _httpClientFactory.CreateClient();
            var tokenResponse = await httpClient.SendAsync(tokenRequest);
            tokenResponse.EnsureSuccessStatusCode();

            var tokenResponseContent = await tokenResponse.Content.ReadFromJsonAsync<JsonElement>();

            var token = new SpotifyToken
            {
                AccessToken = tokenResponseContent.GetProperty("access_token").GetString(),
                RefreshToken = tokenResponseContent.GetProperty("refresh_token").GetString(),
                ExpiryTime = DateTime.UtcNow.AddSeconds(tokenResponseContent.GetProperty("expires_in").GetInt32())
            };

            await _tokenService.SaveTokenAsync(token);
            return token;
        }
        return null;
    }

    public async Task<SpotifyToken> RefreshAccessTokenAsync(string refreshToken)
    {
        var clientId = _spotifySettings.ClientId;
        var clientSecret = _spotifySettings.ClientSecret;

        var tokenRequest = new HttpRequestMessage(HttpMethod.Post, "https://accounts.spotify.com/api/token");
        tokenRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        tokenRequest.Content = new FormUrlEncodedContent(new[]
        {
                new KeyValuePair<string, string>("grant_type", "refresh_token"),
                new KeyValuePair<string, string>("refresh_token", refreshToken),
                new KeyValuePair<string, string>("client_id", clientId),
                new KeyValuePair<string, string>("client_secret", clientSecret)
        });

        var httpClient = _httpClientFactory.CreateClient();
        var tokenResponse = await httpClient.SendAsync(tokenRequest);
        tokenResponse.EnsureSuccessStatusCode();

        var tokenResponseContent = await tokenResponse.Content.ReadFromJsonAsync<JsonElement>();

        var token = new SpotifyToken
        {
            AccessToken = tokenResponseContent.GetProperty("access_token").GetString(),
            RefreshToken = refreshToken,
            ExpiryTime = DateTime.UtcNow.AddSeconds(tokenResponseContent.GetProperty("expires_in").GetInt32())
        };

        await _tokenService.SaveTokenAsync(token);
        return token;
    }

    public async Task<SpotifyToken> GetOrRefreshToken(string code = null)
    {
        var currentToken = await _tokenService.GetTokenAsync();

        if (currentToken == null || currentToken.ExpiryTime <= DateTime.UtcNow)
        {
            if (currentToken == null && !string.IsNullOrEmpty(code))
            {
                currentToken = await GetAccessTokenAsync(code);
            }
            else if (currentToken != null)
            {
                currentToken = await RefreshAccessTokenAsync(currentToken.RefreshToken);
            }
            else
            {
                throw new InvalidOperationException("No valid code or token available.");
            }
        }

        return currentToken;
    }
}
