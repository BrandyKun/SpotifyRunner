using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public class SpotifyDbContext : DbContext 
{
    public SpotifyDbContext(DbContextOptions<SpotifyDbContext> options) : base(options)
    {
        
    }
    public DbSet<SpotifyToken> SpotifyTokens { get; set; }
    public DbSet<ClientDetail> ClientDetails { get; set; }
}
