using Domain.Entities.UserEntities;
using SpotifyAPI.Web;

namespace Application.Interface;

public interface ISpotifyDataService
{
    Task<PrivateUser> GetUserProfile();
    Task<PublicUser> GetUserInfo();
}
