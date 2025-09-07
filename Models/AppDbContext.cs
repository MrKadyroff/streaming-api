using Microsoft.EntityFrameworkCore;
using Models.Entities;

namespace Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Models.Entities.Stream> Streams { get; set; }
        public DbSet<Models.Entities.User> Users { get; set; }
        public DbSet<Models.Entities.Ad> Ads { get; set; }
        public DbSet<Models.Entities.Match> Matches { get; set; }
        public DbSet<Models.Entities.Report> Reports { get; set; }
        // TODO: Add DbSet for other entities (Settings, etc.)
    }
}
