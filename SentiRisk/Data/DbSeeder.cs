using SentiRisk.Models;
using SentiRisk.Data;
using System;
using System.Linq;

namespace SentiRisk.Data
{
    public static class DbSeeder
    {
        public static void Seed(SentiRiskContext context)
        {
            if (context.Role.Any()) return;

            // Roles
            var adminRole = new Role { Name = "Admin" };
            var userRole = new Role { Name = "User" };
            context.Role.AddRange(adminRole, userRole);
            context.SaveChanges();

            // Users
            var alice = new User
            {
                UserName = "alice",
                Email = "alice@example.com",
                PasswordHash = "hashed-password-sample",
                RoleId = adminRole.Id
            };
            var bob = new User
            {
                UserName = "bob",
                Email = "bob@example.com",
                PasswordHash = "hashed-password-sample",
                RoleId = userRole.Id
            };
            context.User.AddRange(alice, bob);
            context.SaveChanges();

            // Assets
            var apple = new Asset
            {
                Name = "Apple Inc.",
                Ticker = "AAPL",
                Sector = "Technology",
                CurrentPrice = 175.50m
            };
            var msft = new Asset
            {
                Name = "Microsoft Corp.",
                Ticker = "MSFT",
                Sector = "Technology",
                CurrentPrice = 324.10m
            };
            context.Asset.AddRange(apple, msft);
            context.SaveChanges();

            // Portfolio
            var alicePortfolio = new Portfolio
            {
                Name = "Alice Portfolio",
                Description = "Exemple de portfolio",
                UserId = alice.Id
            };
            context.Portfolio.Add(alicePortfolio);
            context.SaveChanges();

            // PortfolioAssets
            var pa1 = new PortfolioAsset
            {
                PortfolioId = alicePortfolio.Id,
                AssetId = apple.Id,
                Weight = 0.6m
            };
            var pa2 = new PortfolioAsset
            {
                PortfolioId = alicePortfolio.Id,
                AssetId = msft.Id,
                Weight = 0.4m
            };
            context.PortfolioAsset.AddRange(pa1, pa2);
            context.SaveChanges();

            // News
            var news1 = new News
            {
                Title = "Apple dépasse les attentes",
                Content = "Apple annonce des résultats supérieurs aux attentes.",
                PublishedDate = DateTime.UtcNow.AddDays(-1),
                AssetId = apple.Id
            };
            var news2 = new News
            {
                Title = "Microsoft annonce une acquisition",
                Content = "Microsoft finalise une acquisition stratégique.",
                PublishedDate = DateTime.UtcNow.AddDays(-2),
                AssetId = msft.Id
            };
            context.News.AddRange(news1, news2);
            context.SaveChanges();

            // SentimentScores
            var s1 = new SentimentScore
            {
                Score = 0.8m,
                Confidence = 0.9m,
                NewsId = news1.Id
            };
            var s2 = new SentimentScore
            {
                Score = 0.6m,
                Confidence = 0.85m,
                NewsId = news2.Id
            };
            context.SentimentScore.AddRange(s1, s2);
            context.SaveChanges();

            // Scenario
            var scenario = new Scenario
            {
                ScenarioName = "Choc Pétrolier",
                Description = "Impact sur le secteur énergie",
                ImpactFactor = 0.3m,
                TargetSector = "Energy",
                CreatedAt = DateTime.UtcNow
            };
            context.Scenario.Add(scenario);
            context.SaveChanges();
        }
    }
}   
