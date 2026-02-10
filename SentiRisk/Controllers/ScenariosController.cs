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
    public class ScenariosController : ControllerBase
    {
        private readonly SentiRiskContext _context;

        public ScenariosController(SentiRiskContext context)
        {
            _context = context;
        }

        // GET: api/Scenarios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ApiScenarioDto>>> GetScenario()
        {
            var scenarios = await _context.Scenario.ToListAsync();
            return scenarios.Select(s => new ApiScenarioDto
            {
                Id = s.Id,
                ScenarioName = s.ScenarioName ?? string.Empty,
                Description = s.Description ?? string.Empty,
                ImpactFactor = s.ImpactFactor,
                TargetSector = s.TargetSector ?? string.Empty,
                CreatedAt = s.CreatedAt
            }).ToList();
        }

        // GET: api/Scenarios/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiScenarioDto>> GetScenario(int id)
        {
            var scenario = await _context.Scenario.FindAsync(id);
            if (scenario == null) return NotFound();

            return new ApiScenarioDto
            {
                Id = scenario.Id,
                ScenarioName = scenario.ScenarioName ?? string.Empty,
                Description = scenario.Description ?? string.Empty,
                ImpactFactor = scenario.ImpactFactor,
                TargetSector = scenario.TargetSector ?? string.Empty,
                CreatedAt = scenario.CreatedAt
            };
        }

        // PUT: api/Scenarios/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutScenario(int id, ApiScenarioCreateDto dto)
        {
            if (dto == null) return BadRequest();

            var existing = await _context.Scenario.FindAsync(id);
            if (existing == null) return NotFound();

            existing.ScenarioName = dto.ScenarioName;
            existing.Description = dto.Description;
            existing.ImpactFactor = dto.ImpactFactor;
            existing.TargetSector = dto.TargetSector;

            _context.Entry(existing).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/Scenarios
        [HttpPost]
        public async Task<ActionResult<ApiScenarioDto>> PostScenario(ApiScenarioCreateDto dto)
        {
            if (dto == null) return BadRequest();

            var scenario = new Scenario
            {
                ScenarioName = dto.ScenarioName,
                Description = dto.Description,
                ImpactFactor = dto.ImpactFactor,
                TargetSector = dto.TargetSector,
                CreatedAt = DateTime.Now
            };

            _context.Scenario.Add(scenario);
            await _context.SaveChangesAsync();

            var response = new ApiScenarioDto
            {
                Id = scenario.Id,
                ScenarioName = scenario.ScenarioName ?? string.Empty,
                Description = scenario.Description ?? string.Empty,
                ImpactFactor = scenario.ImpactFactor,
                TargetSector = scenario.TargetSector ?? string.Empty,
                CreatedAt = scenario.CreatedAt
            };

            return CreatedAtAction(nameof(GetScenario), new { id = response.Id }, response);
        }

        // DELETE: api/Scenarios/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteScenario(int id)
        {
            var scenario = await _context.Scenario.FindAsync(id);
            if (scenario == null) return NotFound();

            _context.Scenario.Remove(scenario);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
