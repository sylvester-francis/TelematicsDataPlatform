using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using TelematicsCore.Models;

namespace TelematicsData
{
    public class TelematicsDbContext : DbContext
    {
        public TelematicsDbContext(DbContextOptions<TelematicsDbContext> options) : base(options)
        {
        }

        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<TelematicsEvent> TelematicsEvents { get; set; }
        public DbSet<Trip> Trips { get; set; }
        public DbSet<Alert> Alerts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Vehicle configuration
            modelBuilder.Entity<Vehicle>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.VehicleIdentifier).IsUnique();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
            });

            // TelematicsEvent configuration
            modelBuilder.Entity<TelematicsEvent>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.VehicleId, e.Timestamp });
                entity.HasIndex(e => e.Timestamp);
                entity.HasIndex(e => e.IsProcessed);

                entity.Property(e => e.Location).HasColumnType("geography");
                entity.Property(e => e.ProcessedAt).HasDefaultValueSql("GETUTCDATE()");

                entity.HasOne(e => e.Vehicle)
                    .WithMany(v => v.TelematicsEvents)
                    .HasForeignKey(e => e.VehicleId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Trip configuration
            modelBuilder.Entity<Trip>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.VehicleId, e.StartTime });

                entity.Property(e => e.StartLocation).HasColumnType("geography");
                entity.Property(e => e.EndLocation).HasColumnType("geography");
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

                entity.HasOne(e => e.Vehicle)
                    .WithMany(v => v.Trips)
                    .HasForeignKey(e => e.VehicleId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Alert configuration
            modelBuilder.Entity<Alert>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.VehicleId, e.CreatedAt });
                entity.HasIndex(e => e.IsAcknowledged);

                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

                entity.HasOne(e => e.TelematicsEvent)
                    .WithMany(te => te.Alerts)
                    .HasForeignKey(e => e.TelematicsEventId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Vehicle)
                    .WithMany()
                    .HasForeignKey(e => e.VehicleId)
                    .OnDelete(DeleteBehavior.NoAction);
            });
        }
    }
}