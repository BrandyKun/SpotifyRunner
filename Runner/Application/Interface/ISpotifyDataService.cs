using Domain.Entities;

namespace Application.Interface;

public interface ISpotifyDataService
{
    Task<SpotifyUser> GetUserInfo();
}
