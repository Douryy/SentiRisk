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
    public class UsersController : ControllerBase
    {
        private readonly SentiRiskContext _context;

        public UsersController(SentiRiskContext context)
        {
            _context = context;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ApiUserDto>>> GetUser()
        {
            var users = await _context.User.Include(u => u.Role).ToListAsync();
            return users.Select(u => new ApiUserDto
            {
                Id = u.Id,
                UserName = u.UserName ?? string.Empty,
                Email = u.Email ?? string.Empty,
                Role = u.Role == null ? null : new ApiRoleDto
                {
                    Id = u.Role.Id,
                    Name = u.Role.Name ?? string.Empty
                }
            }).ToList();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiUserDto>> GetUser(int id)
        {
            var user = await _context.User.Include(u => u.Role).Include(u => u.ListePortfolios).FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) return NotFound();

            return new ApiUserDto
            {
                Id = user.Id,
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                Role = user.Role == null ? null : new ApiRoleDto
                {
                    Id = user.Role.Id,
                    Name = user.Role.Name ?? string.Empty
                }
            };
        }

        // PUT: api/Users/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, ApiUserCreateDto dto)
        {
            if (dto == null) return BadRequest();

            var existing = await _context.User.FindAsync(id);
            if (existing == null) return NotFound();

            if (!await _context.Role.AnyAsync(r => r.Id == dto.RoleId))
                return BadRequest("Le RoleId spécifié n'existe pas.");

            // Vérifier unicité email/username si modifiés
            if (await _context.User.AnyAsync(u => u.Id != id && (u.Email == dto.Email || u.UserName == dto.UserName)))
                return Conflict("L'email ou l'username est déjà utilisé.");

            existing.UserName = dto.UserName;
            existing.Email = dto.Email;
            existing.PasswordHash = dto.PasswordHash;
            existing.RoleId = dto.RoleId;

            _context.Entry(existing).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/Users
        [HttpPost]
        public async Task<ActionResult<ApiUserDto>> PostUser(ApiUserCreateDto dto)
        {
            if (dto == null) return BadRequest();

            if (!await _context.Role.AnyAsync(r => r.Id == dto.RoleId))
                return BadRequest("Le RoleId spécifié n'existe pas.");

            if (await _context.User.AnyAsync(u => u.Email == dto.Email || u.UserName == dto.UserName))
                return Conflict("L'utilisateur ou l'email existe déjà.");

            var user = new User
            {
                UserName = dto.UserName,
                Email = dto.Email,
                PasswordHash = dto.PasswordHash,
                RoleId = dto.RoleId
            };

            _context.User.Add(user);
            await _context.SaveChangesAsync();

            var created = await _context.User.Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == user.Id);

            var response = new ApiUserDto
            {
                Id = created!.Id,
                UserName = created.UserName ?? string.Empty,
                Email = created.Email ?? string.Empty,
                Role = created.Role == null ? null : new ApiRoleDto
                {
                    Id = created.Role.Id,
                    Name = created.Role.Name ?? string.Empty
                }
            };

            return CreatedAtAction(nameof(GetUser), new { id = response.Id }, response);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.User.FindAsync(id);
            if (user == null) return NotFound();

            _context.User.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}