using Application.Interface;
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace Infrastructure.Services;

public class SpotifyAuthService : ISpotifyAuthService
{
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string spotifyUrl = "https://accounts.spotify.com/api/token";

    public SpotifyAuthService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }
    public async Task<SpotifyToken> GetAccessTokenAsync(string code)
    {
        var clientId = _configuration["Spotify:ClientId"];
        var clientSecret = _configuration["Spotify:ClientSecret"];
        var redirectUri = _configuration["Spotify:RedirectUri"];

        var tokenRequest = new HttpRequestMessage(HttpMethod.Post, spotifyUrl);
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

        return new SpotifyToken
        {
            AccessToken = tokenResponseContent.GetProperty("access_token").GetString(),
            RefreshToken = tokenResponseContent.GetProperty("refresh_token").GetString(),
            ExpiryTime = DateTime.UtcNow.AddSeconds(tokenResponseContent.GetProperty("expires_in").GetInt32())
        };
    }

    public async Task<SpotifyToken> RefreshAccessTokenAsync(string refreshToken)
    {
        var clientId = _configuration["Spotify:ClientId"];
        var clientSecret = _configuration["Spotify:ClientSecret"];

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

        return new SpotifyToken
        {
            AccessToken = tokenResponseContent.GetProperty("access_token").GetString(),
            RefreshToken = refreshToken,
            ExpiryTime = DateTime.UtcNow.AddSeconds(tokenResponseContent.GetProperty("expires_in").GetInt32())
        };
    }
}
