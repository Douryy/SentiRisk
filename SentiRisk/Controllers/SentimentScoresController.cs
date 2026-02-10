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
    public class SentimentScoresController : ControllerBase
    {
        private readonly SentiRiskContext _context;

        public SentimentScoresController(SentiRiskContext context)
        {
            _context = context;
        }

        // GET: api/SentimentScores
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SentimentScore>>> GetSentimentScore()
        {
            return await _context.SentimentScore.ToListAsync();
        }

        // GET: api/SentimentScores/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SentimentScore>> GetSentimentScore(int id)
        {
            var sentimentScore = await _context.SentimentScore
                .Include(s => s.News)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (sentimentScore == null) return NotFound();

            return sentimentScore;
        }

        // PUT: api/SentimentScores/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSentimentScore(int id, SentimentScore dto)
        {
            if (id != dto.Id) return BadRequest();

            // Validation de la news
            if (!await _context.News.AnyAsync(n => n.Id == dto.NewsId))
                return BadRequest("La News (NewsId) spécifiée n'existe pas.");

            _context.Entry(dto).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SentimentScoreExists(id)) return NotFound();
                throw;
            }

            return NoContent();
        }

        // POST: api/SentimentScores
        [HttpPost]
        public async Task<ActionResult<SentimentScore>> PostSentimentScore(SentimentScore dto)
        {
            if (!await _context.News.AnyAsync(n => n.Id == dto.NewsId))
                return BadRequest("La News (NewsId) spécifiée n'existe pas.");

            if (dto.Confidence < 0 || dto.Confidence > 1)
                return BadRequest("La confiance (Confidence) doit être comprise entre 0 et 1.");

            _context.SentimentScore.Add(dto);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSentimentScore), new { id = dto.Id }, dto);
        }

        // DELETE: api/SentimentScores/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSentimentScore(int id)
        {
            var sentimentScore = await _context.SentimentScore.FindAsync(id);
            if (sentimentScore == null) return NotFound();

            _context.SentimentScore.Remove(sentimentScore);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SentimentScoreExists(int id)
        {
            return _context.SentimentScore.Any(e => e.Id == id);
        }
    }
}