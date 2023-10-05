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

public class SpotifyLogin : ISpotifyLogin, ISpotifyRefreshToken
{
    private readonly HttpClient _httpClient;
    private readonly SpotifyDbContext _spotifyDbContext;
    private  SpotifyTokenService _tokenService;
    private readonly string scopes = "user-read-playback-state user-modify-playback-state user-read-currently-playing user-read-playback-position user-top-read user-read-recently-played playlist-read-private playlist-read-collaborative playlist-modify-private playlist-modify-public user-read-private user-read-email user-library-modify user-library-read";

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

    public async Task<string> GetAuthCodeAsync()
    {

        var spotifyDets = await GetClientDetailsAsync();

        if (spotifyDets == null)
            throw new ArgumentNullException();


        string urlParams = $"response_type=code&state=16&client_id={spotifyDets.ClientId}&redirect_uri=http://localhost:5039&scope={scopes}&show_dialog=true";
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"{authCodeEndpoint}?{urlParams}");

        var messageResponse = await _httpClient.RequestAuthorizationCodeTokenAsync(new AuthorizationCodeTokenRequest()
        {
            Address = tokenEndpoint,
            ClientId = spotifyDets.ClientId,
            ClientSecret = spotifyDets.ClientSecret,
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

        if (authCode == null)
            throw new ArgumentNullException();
            
        return authCode;
    }

    public async Task<AuthResult> ExchangeCodeForAccessToken(string code, string state)
    {
        AuthResult token = new AuthResult();

        var spotifyDets = await GetClientDetailsAsync();

        try
        {
            if (!string.IsNullOrEmpty(code))
            {
                HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, tokenEndpoint);

                requestMessage.Headers.Authorization = new AuthenticationHeaderValue(
                    "Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{spotifyDets.ClientId}:{spotifyDets.ClientSecret}")));

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

                if (token == null)
                    throw new NullReferenceException();

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


    public Uri BuildUri()
    {
        var spotifyDets = GetClientDetailsAsync().GetAwaiter().GetResult();
        string urlParams = $"response_type=code&state=16&client_id={spotifyDets.ClientId}&redirect_uri=http://localhost:5039&scope={scopes}&show_dialog=true";

        StringBuilder sb = new StringBuilder();
        sb.Append("/authorize?");
        sb.Append("response_type=code");
        sb.Append("&state=16");
        sb.Append($"&client_id={spotifyDets.ClientId}");
        sb.Append($"&redirect_uri={Uri.EscapeDataString("http://localhost:3000/callback")}");
        // sb.Append($"&redirect_uri={Uri.EscapeUriString("http://localhost:5039/callback")}");
        sb.Append($"&scope={scopes}");
        // sb.Append("&show_dialog=true");
        return new Uri(new Uri(authCodeEndpoint), sb.ToString());
    }
    Task<SpotifyTokentoReturn> ISpotifyRefreshToken.ReturnAccessTokenFromRefreshToken()
    {
        throw new NotImplementedException();
    }

    // 


}
