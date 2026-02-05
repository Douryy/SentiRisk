using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SentiRisk.Data;
using SentiRisk.Models;

namespace SentiRisk.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PortfolioAssetsController : ControllerBase
    {
        private readonly SentiRiskContext _context;

        public PortfolioAssetsController(SentiRiskContext context)
        {
            _context = context;
        }

        // GET: api/PortfolioAssets
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PortfolioAsset>>> GetPortfolioAsset()
        {
            return await _context.PortfolioAsset.ToListAsync();
        }

        // GET: api/PortfolioAssets/5
        [HttpGet("{portfolioId}/{assetId}")]
        public async Task<ActionResult<PortfolioAsset>> GetPortfolioAsset(int portfolioId ,int assetId)
        {
            var portfolioAsset = await _context.PortfolioAsset.FindAsync(portfolioId, assetId);

            if (portfolioAsset == null)
            {
                return NotFound();
            }

            return portfolioAsset;
        }

        // PUT: api/PortfolioAssets/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
  
        [HttpPut("{portfolioId}/{assetId}")]
        public async Task<IActionResult> PutPortfolioAsset(int portfolioId, int assetId, PortfolioAsset portfolioAsset)
        {
            // On vérifie la cohérence des IDs entre l'URL et le corps de la requête
            if (portfolioId != portfolioAsset.PortfolioId || assetId != portfolioAsset.AssetId)
            {
                return BadRequest("Les IDs ne correspondent pas.");
            }

            _context.Entry(portfolioAsset).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // On utilise les deux IDs ici pour corriger l'erreur CS7036
                if (!PortfolioAssetExists(portfolioId, assetId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/PortfolioAssets
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PortfolioAsset>> PostPortfolioAsset(PortfolioAsset portfolioAsset)
        {
            // 1. On vérifie d'abord que le poids total ne dépasse pas 100% (Règle Métier)
            var currentTotalWeight = await _context.PortfolioAsset
                .Where(pa => pa.PortfolioId == portfolioAsset.PortfolioId)
                .SumAsync(pa => pa.Weight);

            if (currentTotalWeight + portfolioAsset.Weight > 1.0m) // 1.0m = 100%
            {
                return BadRequest("Le poids total du portfolio dépasse 100%.");
            }

            _context.PortfolioAsset.Add(portfolioAsset);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                // Correction ici : On vérifie l'existence avec les DEUX clés
                if (PortfolioAssetExists(portfolioAsset.PortfolioId, portfolioAsset.AssetId))
                {
                    return Conflict("Cet actif existe déjà dans ce portfolio.");
                }
                else
                {
                    throw;
                }
            }

            // Correction ici : On renvoie les deux IDs pour le GetPortfolioAsset
            return CreatedAtAction("GetPortfolioAsset",
                new { portfolioId = portfolioAsset.PortfolioId, assetId = portfolioAsset.AssetId },
                portfolioAsset);
        }

        // DELETE: api/PortfolioAssets/5
        [HttpDelete("{portfolioId}/{assetId}")]
        public async Task<IActionResult> DeletePortfolioAsset(int portfolioId, int assetId)
        {
            var portfolioAsset = await _context.PortfolioAsset.FindAsync(portfolioId, assetId);
            if (portfolioAsset == null)
            {
                return NotFound();
            }

            _context.PortfolioAsset.Remove(portfolioAsset);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PortfolioAssetExists(int pId, int aId)
        {
            return _context.PortfolioAsset.Any(e => e.PortfolioId == pId && e.AssetId == aId);
        }
    }
}
