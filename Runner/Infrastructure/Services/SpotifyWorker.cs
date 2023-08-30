using Application.Interface;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Infrastructure.Services;

public class SpotifyWorker : ISpotifyWorker
{
    private readonly HttpClient _httpClient;
    private readonly SpotifyDbContext _spotifyDbContext;
    private SpotifyTokentoReturn token = new SpotifyTokentoReturn();
    private readonly string tokenEndpoint = "https://accounts.spotify.com/api/token";


    public SpotifyWorker(HttpClient httpClient, SpotifyDbContext spotifyDbContext)
    {
        _httpClient = httpClient;
        _spotifyDbContext = spotifyDbContext;
    }

    public string AccessToken
    {
        get
        {
            if (token.IsValid || ReturnAccessTokenFromRefreshToken().Result)
            {
                return token.AccessToken;
            }
            return string.Empty;
        }

    }
    public Task<string> GetSpotifyInformation()
    {
        throw new NotImplementedException();
    }

    public async Task<bool> ReturnAccessTokenFromRefreshToken()
    {
        if (token.IsValid)
        {
            return true;
        }
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

            return true;

        }
        catch (HttpRequestException ex)
        {
            throw new HttpRequestException(ex.Message, ex);
        }
    }
}
