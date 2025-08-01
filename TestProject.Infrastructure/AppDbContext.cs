using Microsoft.EntityFrameworkCore;
using TestProject.Core.Entities;

namespace TestProject.Infrastructure
{
    public class AppDbContext : DbContext
    {
        public DbSet<Meteorite> Meteorite { get; set; } = null!;
        public DbSet<AppLog> Logs { get; set; } = null!;

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {    
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(
                "Host=localhost;Database=mydb;Username=admin;Password=admin",
                x => x.UseNetTopologySuite()
            );
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.HasPostgresExtension("postgis");

            builder.Entity<Meteorite>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name).IsRequired();

                entity.Property(e => e.Geolocation)
                    .HasColumnType("geometry (point, 4326)");

                entity.Property(e => e.Reclat).HasColumnType("numeric(9,6)");
                entity.Property(e => e.Reclong).HasColumnType("numeric(9,6)");

                entity.Ignore(e => e.CreatedAt);
                entity.Ignore(e => e.UpdatedAt);
            });
        }
    }
}
