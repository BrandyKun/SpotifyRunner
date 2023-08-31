using Application.Interface;
using Domain.Entities;

namespace Infrastructure.Services;

public class SpotifydataService : ISpotifyDataService
{
    private readonly 
    public SpotifydataService()
    {
    }

    public Task<SpotifyUser> GetUserInfo(string token)
    {
        throw new NotImplementedException();
    }
}
