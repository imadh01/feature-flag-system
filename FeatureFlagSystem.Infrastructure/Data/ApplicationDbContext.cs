using FeatureFlagSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FeatureFlagSystem.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<FeatureFlag> FeatureFlags { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure FeatureFlag entity
            var featureFlagEntity = modelBuilder.Entity<FeatureFlag>();

            featureFlagEntity.ToTable("FeatureFlags");
            featureFlagEntity.HasKey(e => e.Id);

            featureFlagEntity.Property(e => e.Id).ValueGeneratedOnAdd();

            featureFlagEntity.Property(e => e.FeatureName)
                .IsRequired()
                .HasMaxLength(100);

            featureFlagEntity.HasIndex(e => e.FeatureName)
                .IsUnique();

            featureFlagEntity.Property(e => e.Description)
                .HasMaxLength(500);

            featureFlagEntity.Property(e => e.IsEnabled)
                .IsRequired();

            featureFlagEntity.Property(e => e.RolloutPercentage)
                .IsRequired();

            featureFlagEntity.Property(e => e.CreatedAt)
                .IsRequired();

            featureFlagEntity.Property(e => e.UpdatedAt)
                .IsRequired();
        }
    }
}