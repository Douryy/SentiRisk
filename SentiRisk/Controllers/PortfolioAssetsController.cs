using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        public async Task<ActionResult<IEnumerable<ApiPortfolioAssetDto>>> GetPortfolioAsset()
        {
            var pas = await _context.PortfolioAsset.Include(pa => pa.Asset).ToListAsync();
            return pas.Select(pa => new ApiPortfolioAssetDto
            {
                Weight = pa.Weight,
                Asset = pa.Asset == null ? null : new ApiAssetDto
                {
                    Id = pa.Asset.Id,
                    Name = pa.Asset.Name ?? string.Empty,
                    Ticker = pa.Asset.Ticker ?? string.Empty,
                    Sector = pa.Asset.Sector ?? string.Empty,
                    CurrentPrice = pa.Asset.CurrentPrice
                }
            }).ToList();
        }

        // GET: api/PortfolioAssets/{portfolioId}/{assetId}
        [HttpGet("{portfolioId}/{assetId}")]
        public async Task<ActionResult<ApiPortfolioAssetDto>> GetPortfolioAsset(int portfolioId, int assetId)
        {
            var pa = await _context.PortfolioAsset.Include(x => x.Asset)
                .FirstOrDefaultAsync(x => x.PortfolioId == portfolioId && x.AssetId == assetId);

            if (pa == null) return NotFound();

            return new ApiPortfolioAssetDto
            {
                Weight = pa.Weight,
                Asset = pa.Asset == null ? null : new ApiAssetDto
                {
                    Id = pa.Asset.Id,
                    Name = pa.Asset.Name ?? string.Empty,
                    Ticker = pa.Asset.Ticker ?? string.Empty,
                    Sector = pa.Asset.Sector ?? string.Empty,
                    CurrentPrice = pa.Asset.CurrentPrice
                }
            };
        }

        // PUT: api/PortfolioAssets/{portfolioId}/{assetId}
        [HttpPut("{portfolioId}/{assetId}")]
        public async Task<IActionResult> PutPortfolioAsset(int portfolioId, int assetId, ApiPortfolioAssetUpsertDto dto)
        {
            if (dto == null) return BadRequest();
            if (portfolioId != dto.PortfolioId || assetId != dto.AssetId) return BadRequest("Les IDs ne correspondent pas.");

            var existing = await _context.PortfolioAsset.FindAsync(portfolioId, assetId);
            if (existing == null) return NotFound();

            // Vérifier règle métier : somme des weights <= 1.0
            var otherTotal = await _context.PortfolioAsset
                .Where(pa => pa.PortfolioId == dto.PortfolioId && !(pa.PortfolioId == dto.PortfolioId && pa.AssetId == dto.AssetId))
                .SumAsync(pa => pa.Weight);

            if (otherTotal + dto.Weight > 1.0m) return BadRequest("Le poids total du portfolio dépasse 100%.");

            existing.Weight = dto.Weight;
            _context.Entry(existing).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/PortfolioAssets
        [HttpPost]
        public async Task<ActionResult<ApiPortfolioAssetDto>> PostPortfolioAsset(ApiPortfolioAssetUpsertDto dto)
        {
            if (dto == null) return BadRequest();

            // Validation existence portfolio & asset
            if (!await _context.Portfolio.AnyAsync(p => p.Id == dto.PortfolioId))
                return BadRequest("Le portfolio spécifié n'existe pas.");
            if (!await _context.Asset.AnyAsync(a => a.Id == dto.AssetId))
                return BadRequest("L'actif spécifié n'existe pas.");

            var currentTotal = await _context.PortfolioAsset
                .Where(pa => pa.PortfolioId == dto.PortfolioId)
                .SumAsync(pa => pa.Weight);

            if (currentTotal + dto.Weight > 1.0m) return BadRequest("Le poids total du portfolio dépasse 100%.");

            if (await _context.PortfolioAsset.AnyAsync(pa => pa.PortfolioId == dto.PortfolioId && pa.AssetId == dto.AssetId))
                return Conflict("Cet actif existe déjà dans ce portfolio.");

            var paEntity = new PortfolioAsset
            {
                PortfolioId = dto.PortfolioId,
                AssetId = dto.AssetId,
                Weight = dto.Weight
            };

            _context.PortfolioAsset.Add(paEntity);
            await _context.SaveChangesAsync();

            var pa = await _context.PortfolioAsset.Include(x => x.Asset)
                .FirstOrDefaultAsync(x => x.PortfolioId == dto.PortfolioId && x.AssetId == dto.AssetId);

            var response = new ApiPortfolioAssetDto
            {
                Weight = pa!.Weight,
                Asset = pa.Asset == null ? null : new ApiAssetDto
                {
                    Id = pa.Asset.Id,
                    Name = pa.Asset.Name ?? string.Empty,
                    Ticker = pa.Asset.Ticker ?? string.Empty,
                    Sector = pa.Asset.Sector ?? string.Empty,
                    CurrentPrice = pa.Asset.CurrentPrice
                }
            };

            return CreatedAtAction(nameof(GetPortfolioAsset), new { portfolioId = dto.PortfolioId, assetId = dto.AssetId }, response);
        }

        // DELETE: api/PortfolioAssets/{portfolioId}/{assetId}
        [HttpDelete("{portfolioId}/{assetId}")]
        public async Task<IActionResult> DeletePortfolioAsset(int portfolioId, int assetId)
        {
            var portfolioAsset = await _context.PortfolioAsset.FindAsync(portfolioId, assetId);
            if (portfolioAsset == null) return NotFound();

            _context.PortfolioAsset.Remove(portfolioAsset);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
