using Domain.Entities;

namespace Application.Interface;

public interface ISpotifyRefreshToken
{
    Task<SpotifyTokentoReturn> ReturnAccessTokenFromRefreshToken();
}
