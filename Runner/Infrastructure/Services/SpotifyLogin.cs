using System.Text;
using System.Dynamic;
using System;
using System.Net.Http;
using Application.Interface;

namespace Infrastructure.Services;

public class SpotifyLogin : ISpotifyLogin
{
    private readonly HttpClient _httpClient;
    public SpotifyLogin(HttpClient httpClient)
    {
        _httpClient = httpClient;

    }
    public async Task<string> ISpotifyLogin.getToken(string clientId, string clientSecret)
    {
        try
        {
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, "token");

            requestMessage.Headers.Authorization = new System.Net.Http.AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}")));
            requestMessage.Content = new FromUrlEncodedContent(new Dictionary<string, string> {
                {"grant-type", "client_credential"}
            });

            var response = await _httpClient.SendAsync(requestMessage);

            response.EnsureSuccessStatusCode();
            var responseData = await response.Content.ReadAsStringAsync();

            var authResult = await JsonSerializer.Deserialize<AuthResult>(responseData);

            return authResult.AccessToken;
        }
        catch
        {

        }
    }
}
