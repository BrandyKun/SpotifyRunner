using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Application.Interface;
using AutoMapper;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class SpotifyRefreshTokenService : ISpotifyRefreshToken
{
    private readonly HttpClient _httpClient;
    private readonly IMapper _mapper;
    private readonly SpotifyDbContext _spotifyDbContext;
    private readonly string tokenEndpoint = "https://accounts.spotify.com/api/token";
    public SpotifyRefreshTokenService(HttpClient httpClient, SpotifyDbContext spotifyDbContext, IMapper mapper)
    {
        _spotifyDbContext = spotifyDbContext;
        _httpClient = httpClient;
        _mapper = mapper;

    }

    public async Task<SpotifyTokentoReturn> ReturnAccessTokenFromRefreshToken()
    {
        try
        {
            var token = await _spotifyDbContext.SpotifyTokens.OrderByDescending(t => t.Expirytime)
                                                                .FirstOrDefaultAsync();

            if (token == null)
                throw new ArgumentNullException("token");

            SpotifyTokentoReturn transformedtoken = _mapper.Map<SpotifyToken, SpotifyTokentoReturn>(token);

            if(transformedtoken.IsValid)

            return transformedtoken;

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

            return _mapper.Map<SpotifyTokentoReturn>(newToken);

        }
        catch (HttpRequestException ex)
        {
            throw new HttpRequestException(ex.Message, ex);
        }
    }
}
