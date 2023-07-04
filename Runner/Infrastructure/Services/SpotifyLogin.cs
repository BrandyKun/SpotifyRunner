using System.Text;
using System.Dynamic;
using System;
using System.Net.Http;
using Application.Interface;
using System.Text.Json;

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
        string token ="";
        try
        {
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, "token");

            requestMessage.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}")));
            requestMessage.Content = new FormUrlEncodedContent(new Dictionary<string, string> {
                {"grant-type", "client_credential"}
            });

            var response = await _httpClient.SendAsync(requestMessage);

            response.EnsureSuccessStatusCode();
            var responseData = await response.Content.ReadAsStreamAsync();

            var authResult = await JsonSerializer.DeserializeAsync<Domain.Entities.AuthResult>(responseData);

            return token = authResult.AccessToken;
        }
        catch(HttpRequestException ex) 
        {

        }

        return token;
    }
}
