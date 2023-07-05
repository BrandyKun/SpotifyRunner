using Application.Interface;
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
    public SpotifyLogin(HttpClient httpClient)
    {
        _httpClient = httpClient;

    }
    public async Task<string> GetToken(string clientId, string clientSecret)
    {
        string token = "";
        try
        {
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, "token");

            requestMessage.Headers.Authorization = new AuthenticationHeaderValue(
                "Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}")));

            requestMessage.Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                {"grant_type", "client_credentials"}
            });

            var response = await _httpClient.SendAsync(requestMessage);

            response.EnsureSuccessStatusCode();
            var responseData = await response.Content.ReadAsStreamAsync();

            var authResult = await JsonSerializer.DeserializeAsync<Domain.Entities.AuthResult>(responseData);

            token = authResult?.access_token;
        }
        catch (HttpRequestException ex)
        {
            throw new HttpRequestException(ex.Message, ex);
        }

        return token;
    }
}
