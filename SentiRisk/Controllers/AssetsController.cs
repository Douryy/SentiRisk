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
    public class AssetsController : ControllerBase
    {
        private readonly SentiRiskContext _context;

        public AssetsController(SentiRiskContext context)
        {
            _context = context;
        }

        // GET: api/Assets
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ApiAssetDto>>> GetAsset()
        {
            var assets = await _context.Asset.ToListAsync();
            return assets.Select(a => new ApiAssetDto
            {
                Id = a.Id,
                Name = a.Name ?? string.Empty,
                Ticker = a.Ticker ?? string.Empty,
                Sector = a.Sector ?? string.Empty,
                CurrentPrice = a.CurrentPrice
            }).ToList();
        }

        // GET: api/Assets/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiAssetDto>> GetAsset(int id)
        {
            var asset = await _context.Asset
                  .Include(a => a.ListeNews!)
                      .ThenInclude(n => n.ListeSentimentScores)
                  .FirstOrDefaultAsync(a => a.Id == id);

            if (asset == null) return NotFound();

            return new ApiAssetDto
            {
                Id = asset.Id,
                Name = asset.Name ?? string.Empty,
                Ticker = asset.Ticker ?? string.Empty,
                Sector = asset.Sector ?? string.Empty,
                CurrentPrice = asset.CurrentPrice
            };
        }

        // PUT: api/Assets/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAsset(int id, ApiAssetCreateDto dto)
        {
            if (dto == null) return BadRequest();

            var existing = await _context.Asset.FindAsync(id);
            if (existing == null) return NotFound();

            existing.Name = dto.Name;
            existing.Ticker = dto.Ticker?.ToUpper();
            existing.Sector = dto.Sector;
            existing.CurrentPrice = dto.CurrentPrice;

            _context.Entry(existing).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AssetExists(id)) return NotFound();
                throw;
            }

            return NoContent();
        }

        // POST: api/Assets
        [HttpPost]
        public async Task<ActionResult<ApiAssetDto>> PostAsset(ApiAssetCreateDto dto)
        {
            if (dto == null) return BadRequest();

            var ticker = dto.Ticker?.ToUpper();

            if (dto.CurrentPrice < 0) return BadRequest("Le prix ne peut pas être négatif.");
            if (await _context.Asset.AnyAsync(a => a.Ticker == ticker))
                return Conflict("Cet actif existe déjà (Ticker en double).");

            var asset = new Asset
            {
                Name = dto.Name,
                Ticker = ticker,
                Sector = dto.Sector,
                CurrentPrice = dto.CurrentPrice
            };

            _context.Asset.Add(asset);
            await _context.SaveChangesAsync();

            var response = new ApiAssetDto
            {
                Id = asset.Id,
                Name = asset.Name ?? string.Empty,
                Ticker = asset.Ticker ?? string.Empty,
                Sector = asset.Sector ?? string.Empty,
                CurrentPrice = asset.CurrentPrice
            };

            return CreatedAtAction(nameof(GetAsset), new { id = asset.Id }, response);
        }

        // DELETE: api/Assets/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsset(int id)
        {
            var asset = await _context.Asset.FindAsync(id);
            if (asset == null) return NotFound();

            var isUsed = await _context.PortfolioAsset.AnyAsync(pa => pa.AssetId == id);
            if (isUsed) return BadRequest("Impossible de supprimer : l'actif est présent dans un portfolio.");

            _context.Asset.Remove(asset);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AssetExists(int id)
        {
            return _context.Asset.Any(e => e.Id == id);
        }
    }
}
