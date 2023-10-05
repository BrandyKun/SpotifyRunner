using Domain.Entities;

namespace Application.Interface;

public interface ISpotifyTokenService
{
    Task<SpotifyTokentoReturn> GetTokenFromDB();
    Task<ClientDetail> GetClientDetails();
}
