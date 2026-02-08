using SentiRisk.Data;
using Microsoft.EntityFrameworkCore;

namespace SentiRisk.Services
{
    public class StressTestService
    {
        
        
            private readonly SentiRiskContext _context;

            public StressTestService(SentiRiskContext context)
            {
                _context = context;
            }

            public async Task<object> SimulateScenario(int portfolioId, int scenarioId)
            {
                var portfolio = await _context.Portfolio .Include(p => p.ListePortfolioAssets).ThenInclude(pa => pa.Asset)  .FirstOrDefaultAsync(p => p.Id == portfolioId);

                var scenario = await _context.Scenario.FindAsync(scenarioId);

                if (portfolio == null || scenario == null) return null;

                decimal initialValue = 0;
                decimal stressedValue = 0;

                foreach (var pa in portfolio.ListePortfolioAssets)
                {
                    var value = pa.Asset.CurrentPrice * (decimal)pa.Weight;
                    initialValue += value;

                    // Si l'actif appartient au secteur visé par le crash
                    if (pa.Asset.Sector == scenario.TargetSector)
                    {
                        stressedValue += value * (1 + scenario.ImpactFactor);
                    }
                    else
                    {
                        stressedValue += value;
                    }
                }

                return new
                {
                    Scenario = scenario.ScenarioName,
                    InitialValue = initialValue,
                    StressedValue = stressedValue,
                    Loss = initialValue - stressedValue
                };
            }
        }
    }

