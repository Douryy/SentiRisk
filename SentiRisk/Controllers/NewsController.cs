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
    public class NewsController : ControllerBase
    {
        private readonly SentiRiskContext _context;

        public NewsController(SentiRiskContext context)
        {
            _context = context;
        }

        // GET: api/News
        [HttpGet]
        public async Task<ActionResult<IEnumerable<News>>> GetNews()
        {
            return await _context.News
                .Include(n => n.Asset)
                .Include(n => n.ListeSentimentScores)
                .ToListAsync();
        }

        // GET: api/News/5
        [HttpGet("{id}")]
        public async Task<ActionResult<News>> GetNews(int id)
        {
            var news = await _context.News
                .Include(n => n.Asset)
                .Include(n => n.ListeSentimentScores)
                .FirstOrDefaultAsync(n => n.Id == id);

            if (news == null)
            {
                return NotFound();
            }

            return news;
        }

        // PUT: api/News/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutNews(int id, News news)
        {
            if (id != news.Id)
            {
                return BadRequest();
            }

            _context.Entry(news).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NewsExists(id))
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

        // POST: api/News
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<News>> PostNews(News news)
        {
            // Vérification : l'actif doit exister
            if (!await _context.Asset.AnyAsync(a => a.Id == news.AssetId))
            {
                return BadRequest("L'actif (AssetId) spécifié n'existe pas.");
            }

            // Si la date de publication n'est pas fournie, utiliser now
            if (news.PublishedDate == default)
            {
                news.PublishedDate = DateTime.Now;
            }

            _context.News.Add(news);
            await _context.SaveChangesAsync();

            // Recharger pour inclure les relations si besoin
            var created = await _context.News
                .Include(n => n.Asset)
                .Include(n => n.ListeSentimentScores)
                .FirstOrDefaultAsync(n => n.Id == news.Id);

            return CreatedAtAction("GetNews", new { id = news.Id }, created);
        }

        // DELETE: api/News/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNews(int id)
        {
            var news = await _context.News.FindAsync(id);
            if (news == null)
            {
                return NotFound();
            }

            // Supprimer les scores de sentiment associés (si cascade non configurée)
            var scores = await _context.SentimentScore.Where(s => s.NewsId == id).ToListAsync();
            if (scores.Any())
            {
                _context.SentimentScore.RemoveRange(scores);
            }

            _context.News.Remove(news);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool NewsExists(int id)
        {
            return _context.News.Any(e => e.Id == id);
        }
    }
}