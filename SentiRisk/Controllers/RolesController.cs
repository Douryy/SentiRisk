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
    public class RolesController : ControllerBase
    {
        private readonly SentiRiskContext _context;

        public RolesController(SentiRiskContext context)
        {
            _context = context;
        }

        // GET: api/Roles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ApiRoleDto>>> GetRole()
        {
            var roles = await _context.Role.ToListAsync();
            return roles.Select(r => new ApiRoleDto
            {
                Id = r.Id,
                Name = r.Name ?? string.Empty
            }).ToList();
        }

        // GET: api/Roles/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiRoleDto>> GetRole(int id)
        {
            var role = await _context.Role
                .Include(r => r.ListeUsers)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (role == null)
            {
                return NotFound();
            }

            return new ApiRoleDto
            {
                Id = role.Id,
                Name = role.Name ?? string.Empty
            };
        }

        // PUT: api/Roles/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRole(int id, ApiRoleCreateDto dto)
        {
            if (dto == null)
            {
                return BadRequest();
            }

            var existing = await _context.Role.FindAsync(id);
            if (existing == null)
            {
                return NotFound();
            }

            // Empêcher les doublons de noms de rôles (sauf si c'est le même rôle)
            var duplicate = await _context.Role.AnyAsync(r => r.Name == dto.Name && r.Id != id);
            if (duplicate)
            {
                return Conflict("Un rôle avec ce nom existe déjà.");
            }

            existing.Name = dto.Name;

            _context.Entry(existing).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RoleExists(id))
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

        // POST: api/Roles
        [HttpPost]
        public async Task<ActionResult<ApiRoleDto>> PostRole(ApiRoleCreateDto dto)
        {
            if (dto == null)
            {
                return BadRequest();
            }

            // 1. Empêcher les doublons de noms de rôles (ex: deux rôles "Admin")
            if (await _context.Role.AnyAsync(r => r.Name == dto.Name))
            {
                return Conflict("Un rôle avec ce nom existe déjà.");
            }

            var role = new Role
            {
                Name = dto.Name
            };

            _context.Role.Add(role);
            await _context.SaveChangesAsync();

            var responseDto = new ApiRoleDto
            {
                Id = role.Id,
                Name = role.Name ?? string.Empty
            };

            return CreatedAtAction("GetRole", new { id = responseDto.Id }, responseDto);
        }

        // DELETE: api/Roles/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            var role = await _context.Role.FindAsync(id);
            if (role == null)
            {
                return NotFound();
            }

            // 2. Vérification de la dépendance User
            var hasUsers = await _context.User.AnyAsync(u => u.RoleId == id);
            if (hasUsers)
            {
                return BadRequest("Impossible de supprimer ce rôle car il est utilisé par des utilisateurs.");
            }

            _context.Role.Remove(role);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RoleExists(int id)
        {
            return _context.Role.Any(e => e.Id == id);
        }
    }
}
