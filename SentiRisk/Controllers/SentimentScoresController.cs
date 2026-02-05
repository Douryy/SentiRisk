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
            var sentimentScore = await _context.SentimentScore.FindAsync(id);

            if (sentimentScore == null)
            {
                return NotFound();
            }

            return sentimentScore;
        }

        // PUT: api/SentimentScores/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSentimentScore(int id, SentimentScore sentimentScore)
        {
            if (id != sentimentScore.Id)
            {
                return BadRequest();
            }

            _context.Entry(sentimentScore).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SentimentScoreExists(id))
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

        // POST: api/SentimentScores
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<SentimentScore>> PostSentimentScore(SentimentScore sentimentScore)
        {
            _context.SentimentScore.Add(sentimentScore);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSentimentScore", new { id = sentimentScore.Id }, sentimentScore);
        }

        // DELETE: api/SentimentScores/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSentimentScore(int id)
        {
            var sentimentScore = await _context.SentimentScore.FindAsync(id);
            if (sentimentScore == null)
            {
                return NotFound();
            }

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
