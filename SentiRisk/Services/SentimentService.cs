using SentiRisk.Data;
using Microsoft.EntityFrameworkCore;

namespace SentiRisk.Services
{
    public class SentimentService
    {
        private readonly SentiRiskContext _context;

        public SentimentService(SentiRiskContext context)
        {
            _context = context;
        }

        // A. Agrégation pour UN actif (Nvidia)
        public async Task<decimal> GetAverageSentiment(int assetId)
        {
            var scores = await _context.SentimentScore
                .Where(s => s.News.AssetId == assetId)
                .ToListAsync();

            if (scores == null || !scores.Any()) return 0;

            decimal totalWeightedScore = scores.Sum(s => s.Score * s.Confidence);
            decimal totalConfidence = scores.Sum(s => s.Confidence);

            return totalConfidence > 0 ? totalWeightedScore / totalConfidence : 0;
        }

        // B. Agrégation pour le PORTFOLIO (Météo Globale)
        public async Task<object> GetPortfolioWeather(int portfolioId)
        {
            var portfolio = await _context.Portfolio
                .Include(p => p.ListePortfolioAssets)
                .ThenInclude(pa => pa.Asset)
                .FirstOrDefaultAsync(p => p.Id == portfolioId);

            if (portfolio == null) return null;

            decimal globalScore = 0;
            decimal totalWeight = 0;

            foreach (var item in portfolio.ListePortfolioAssets)
            {
                var assetScore = await GetAverageSentiment(item.AssetId);
                globalScore += assetScore * (decimal)item.Weight; // Pondération par le poids du portfolio
                totalWeight += (decimal)item.Weight;
            }

            decimal finalScore = totalWeight > 0 ? globalScore / totalWeight : 0;

            return new
            {
                PortfolioName = portfolio.Name,
                GlobalScore = finalScore,
                Weather = finalScore < -0.2m ? "Orageux (Risque)" : finalScore > 0.2m ? "Ensoleillé" : "Nuageux",
                LastUpdated = DateTime.Now
            };
        }
    }
}
