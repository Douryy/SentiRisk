using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SentiRisk.Data;
using SentiRisk.Models;
using SentiRisk.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SentiRisk.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PortfoliosController : ControllerBase
    {
        private readonly SentiRiskContext _context;
        private readonly PortfolioCalculationService _calcService;
        private readonly StressTestService _stressService;
        private readonly SentimentService _sentimentService;


        public PortfoliosController(SentiRiskContext context, PortfolioCalculationService calcService, StressTestService stressService, SentimentService sentimentService)
        {
            _context = context;
            _calcService = calcService; // On lie le service injecté à notre variable
            _stressService = stressService;
            _sentimentService = sentimentService;
        }

        // GET: api/Portfolios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Portfolio>>> GetPortfolio()
        {
            return await _context.Portfolio.ToListAsync();
        }

        // GET: api/Portfolios/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Portfolio>> GetPortfolio(int id)
        {
            // var portfolio = await _context.Portfolio.FindAsync(id);
            var portfolio = await _context.Portfolio.Include(p => p.User).Include(p => p.ListePortfolioAssets!).ThenInclude(pa => pa.Asset).FirstOrDefaultAsync(p => p.Id == id);

            if (portfolio == null)
            {
                return NotFound();
            }

            return portfolio;
        }
        [HttpGet("{id}/value")]
        public async Task<ActionResult> GetValue(int id)
        {
            var result = await _calcService.GetPortfolioValue(id);
            if (result == null) return NotFound();
            return Ok(result);
        }
        [HttpGet("{id}/simulate/{scenarioId}")]
        public async Task<ActionResult> Simulate(int id, int scenarioId)
        {
            var result = await _stressService.SimulateScenario(id, scenarioId);

            if (result == null)
            {
                return NotFound("Portfolio ou Scénario introuvable.");
            }

            return Ok(result);
        }
        [HttpGet("{id}/weather")]
        public async Task<ActionResult> GetWeather(int id)
        {
            var weather = await _sentimentService.GetPortfolioWeather(id);
            if (weather == null) return NotFound();
            return Ok(weather);
        }
        // PUT: api/Portfolios/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPortfolio(int id, Portfolio portfolio)
        {
            if (id != portfolio.Id)
            {
                return BadRequest();
            }

            _context.Entry(portfolio).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PortfolioExists(id))
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

        // POST: api/Portfolios
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Portfolio>> PostPortfolio(Portfolio portfolio)
        {
            // Vérification : l'utilisateur propriétaire doit exister
            if (!await _context.User.AnyAsync(u => u.Id == portfolio.UserId))
            {
                return BadRequest("L'utilisateur (UserId) spécifié n'existe pas.");
            }

            _context.Portfolio.Add(portfolio);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPortfolio", new { id = portfolio.Id }, portfolio);
        }

        // DELETE: api/Portfolios/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePortfolio(int id)
        {
            var portfolio = await _context.Portfolio.FindAsync(id);
            if (portfolio == null)
            {
                return NotFound();
            }

            _context.Portfolio.Remove(portfolio);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PortfolioExists(int id)
        {
            return _context.Portfolio.Any(e => e.Id == id);
        }
    }
}
