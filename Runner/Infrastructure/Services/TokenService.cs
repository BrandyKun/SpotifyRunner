using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class TokenService
{
    private readonly SpotifyDbContext _context;

    public TokenService(SpotifyDbContext context)
    {
        _context = context;
    }

    public async Task<SpotifyToken> GetTokenAsync()
    {
        return await _context.SpotifyTokens.FirstOrDefaultAsync();
    }

    public async Task SaveTokenAsync(SpotifyToken token)
    {
        var existingToken = await _context.SpotifyTokens.FirstOrDefaultAsync();
        if (existingToken != null)
        {
            existingToken.AccessToken = token.AccessToken;
            existingToken.RefreshToken = token.RefreshToken;
            existingToken.ExpiryTime = token.ExpiryTime;
        }
        else
        {
            _context.SpotifyTokens.Add(token);
        }
        await _context.SaveChangesAsync();
    }
}
