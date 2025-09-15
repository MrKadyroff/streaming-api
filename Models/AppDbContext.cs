using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
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
        public DbSet<UserCounter> UserCounters { get; set; }
        // TODO: Add DbSet for other entities (Settings, etc.)

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Normalize all DateTime values to UTC to satisfy Npgsql timestamptz requirements
            var dateTimeConverter = new ValueConverter<DateTime, DateTime>(
                v => v.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(v, DateTimeKind.Utc) : v.ToUniversalTime(),
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

            var nullableDateTimeConverter = new ValueConverter<DateTime?, DateTime?>(
                v => v.HasValue
                    ? (v.Value.Kind == DateTimeKind.Unspecified
                        ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc)
                        : v.Value.ToUniversalTime())
                    : v,
                v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v);

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(DateTime))
                    {
                        property.SetValueConverter(dateTimeConverter);
                    }
                    else if (property.ClrType == typeof(DateTime?))
                    {
                        property.SetValueConverter(nullableDateTimeConverter);
                    }
                }
            }

            base.OnModelCreating(modelBuilder);
        }
    }
}
