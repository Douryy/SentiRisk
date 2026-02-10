
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
    public class PortfoliosController : ControllerBase
    {
        private readonly SentiRiskContext _context;

        public PortfoliosController(SentiRiskContext context)
        {
            _context = context;
        }

        // GET: api/Portfolios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ApiPortfolioDto>>> GetPortfolio()
        {
            var portfolios = await _context.Portfolio
                .Include(p => p.ListePortfolioAssets!)
                    .ThenInclude(pa => pa.Asset)
                .ToListAsync();

            return portfolios.Select(MapToApi).ToList();
        }

        // GET: api/Portfolios/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiPortfolioDto>> GetPortfolio(int id)
        {
            var portfolio = await _context.Portfolio
                .Include(p => p.User)
                .Include(p => p.ListePortfolioAssets!)
                    .ThenInclude(pa => pa.Asset)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (portfolio == null)
            {
                return NotFound();
            }

            return MapToApi(portfolio);
        }

        // PUT: api/Portfolios/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPortfolio(int id, ApiPortfolioCreateDto dto)
        {
            if (dto == null) return BadRequest();

            var existing = await _context.Portfolio
                .Include(p => p.ListePortfolioAssets!)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (existing == null) return NotFound();

            if (!await _context.User.AnyAsync(u => u.Id == dto.UserId))
            {
                return BadRequest("L'utilisateur (UserId) spécifié n'existe pas.");
            }

            existing.Name = dto.Name;
            existing.Description = dto.Description;

            // Replace assets collection (simplifié)
            existing.ListePortfolioAssets ??= new List<PortfolioAsset>();
            existing.ListePortfolioAssets.Clear();
            if (dto.PortfolioAssets != null)
            {
                foreach (var pa in dto.PortfolioAssets)
                {
                    existing.ListePortfolioAssets.Add(new PortfolioAsset
                    {
                        PortfolioId = existing.Id,
                        AssetId = pa.AssetId,
                        Weight = pa.Weight
                    });
                }
            }

            _context.Entry(existing).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PortfolioExists(id)) return NotFound();
                throw;
            }

            return NoContent();
        }

        // POST: api/Portfolios
        [HttpPost]
        public async Task<ActionResult<ApiPortfolioDto>> PostPortfolio(ApiPortfolioCreateDto dto)
        {
            if (dto == null) return BadRequest();

            if (!await _context.User.AnyAsync(u => u.Id == dto.UserId))
            {
                return BadRequest("L'utilisateur (UserId) spécifié n'existe pas.");
            }

            var portfolio = new Portfolio
            {
                Name = dto.Name,
                Description = dto.Description,
                UserId = dto.UserId,
                ListePortfolioAssets = dto.PortfolioAssets?.Select(pa => new PortfolioAsset
                {
                    AssetId = pa.AssetId,
                    Weight = pa.Weight
                }).ToList()
            };

            _context.Portfolio.Add(portfolio);
            await _context.SaveChangesAsync();

            var created = await _context.Portfolio
                .Include(p => p.ListePortfolioAssets!)
                    .ThenInclude(pa => pa.Asset)
                .FirstOrDefaultAsync(p => p.Id == portfolio.Id);

            return CreatedAtAction(nameof(GetPortfolio), new { id = portfolio.Id }, MapToApi(created!));
        }

        // DELETE: api/Portfolios/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePortfolio(int id)
        {
            var portfolio = await _context.Portfolio.FindAsync(id);
            if (portfolio == null) return NotFound();

            _context.Portfolio.Remove(portfolio);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PortfolioExists(int id)
        {
            return _context.Portfolio.Any(e => e.Id == id);
        }

        // Mapping helper
        private static ApiPortfolioDto MapToApi(Portfolio p)
        {
            var dto = new ApiPortfolioDto
            {
                Id = p.Id,
                Name = p.Name ?? string.Empty,
                Description = p.Description ?? string.Empty,
                PortfolioAssets = new List<ApiPortfolioAssetDto>()
            };

            if (p.ListePortfolioAssets != null)
            {
                foreach (var pa in p.ListePortfolioAssets)
                {
                    dto.PortfolioAssets!.Add(new ApiPortfolioAssetDto
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
                    });
                }
            }

            return dto;
        }
    }
}
