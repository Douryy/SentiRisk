using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SentiRisk.Models;

namespace SentiRisk.Data
{
    public class SentiRiskContext : DbContext
    {
        public SentiRiskContext(DbContextOptions<SentiRiskContext> options)
            : base(options)
        {
        }

        public DbSet<SentiRisk.Models.Role> Role { get; set; } = default!;
        public DbSet<SentiRisk.Models.User> User { get; set; } = default!;
        public DbSet<SentiRisk.Models.Portfolio> Portfolio { get; set; } = default!;
        public DbSet<SentiRisk.Models.Asset> Asset { get; set; } = default!;
        public DbSet<SentiRisk.Models.PortfolioAsset> PortfolioAsset { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PortfolioAsset>()
                .HasKey(pa => new { pa.PortfolioId, pa.AssetId });
            modelBuilder.Entity<PortfolioAsset>()
                .HasOne(pa => pa.Portfolio)
                .WithMany(p => p.ListePortfolioAssets)
                .HasForeignKey(pa => pa.PortfolioId);
            modelBuilder.Entity<PortfolioAsset>()
                .HasOne(pa => pa.Asset)
                .WithMany(a => a.ListePortfolioAssets)
                .HasForeignKey(pa => pa.AssetId);
        }
        public DbSet<SentiRisk.Models.News> News { get; set; } = default!;
        public DbSet<SentiRisk.Models.SentimentScore> SentimentScore { get; set; } = default!;
        public DbSet<SentiRisk.Models.Scenario> Scenario { get; set; } = default!;
    }
}
