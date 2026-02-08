using SentiRisk.Data;
using Microsoft.EntityFrameworkCore;

namespace SentiRisk.Services
{
    public class PortfolioCalculationService
    {
        private readonly SentiRiskContext _context;

        public PortfolioCalculationService(SentiRiskContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Calcule la valeur totale d'un portfolio basé sur (Prix de l'Asset * Poids)
        /// </summary>
        public async Task<object?> GetPortfolioValue(int portfolioId)
        {
            var portfolio = await _context.Portfolio
                .Include(p => p.ListePortfolioAssets!)
                .ThenInclude(pa => pa.Asset)
                .FirstOrDefaultAsync(p => p.Id == portfolioId);

            if (portfolio == null) return null;

            decimal totalValue = 0;
            var details = new List<object>();

            // On boucle sur les actifs pour calculer l'exposition de chaque ligne
            foreach (var item in portfolio.ListePortfolioAssets!)
            {
                var price = item.Asset?.CurrentPrice ?? 0;
                var exposure = price * item.Weight;
                totalValue += exposure;

                details.Add(new
                {
                    AssetName = item.Asset?.Name,
                    Sector = item.Asset?.Sector,
                    Weight = item.Weight,
                    CurrentPrice = price,
                    LineValue = exposure
                });
            }

            return new
            {
                PortfolioName = portfolio.Name,
                TotalValue = totalValue,
                AssetsCount = details.Count,
                Composition = details
            };
        }
    }
}
