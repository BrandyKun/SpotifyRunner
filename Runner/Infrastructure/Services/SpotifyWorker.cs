using Application.Interface;
using Domain.Entities;

namespace Infrastructure.Services;

public class SpotifyWorker : ISpotifyWorker
{
    private SpotifyTokentoReturn token = new SpotifyTokentoReturn();
    public SpotifyWorker()
    {
        
    }
    public string AccessToken 
    {
        get {
            if(token.IsValid || ReturnAccessTokenFromRefreshToken())
        }
    }
    public Task<string> GetSpotifyInformation()
    {
        throw new NotImplementedException();
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
