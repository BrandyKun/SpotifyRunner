using AutoMapper;
using Domain.Entities;

namespace API.mapper;

public class TokenMapping : Profile
{
    public TokenMapping()
    {
        CreateMap<AuthResult, SpotifyToken>()
            .ForMember(x => x.AccessToken,
                        s => s.MapFrom(sp => sp.access_token))
            .ForMember(x => x.RefreshToken,
                        s => s.MapFrom(sp => sp.refresh_token));
    }
}
