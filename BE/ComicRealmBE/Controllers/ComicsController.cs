using ComicRealmBE.Data;
using ComicRealmBE.Dtos;
using ComicRealmBE.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ComicRealmBE.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ComicsController : ControllerBase
    {
        private readonly ComicRealmContext _context;

        public ComicsController(ComicRealmContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize(Roles = "Friend,Admin,SuperAdmin")]
        public async Task<ActionResult<IEnumerable<ComicDto>>> GetAll()
        {
            var comics = await _context.Comics
                .AsNoTracking()
                .Select(c => new ComicDto
                {
                    ComicId = c.ComicId,
                    Serie = c.Serie,
                    Number = c.Number,
                    Title = c.Title,
                    CreatedBy = c.CreatedBy,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt
                })
                .ToListAsync();

            return Ok(comics);
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = "Friend,Admin,SuperAdmin")]
        public async Task<ActionResult<ComicDto>> GetById(int id)
        {
            var comic = await _context.Comics
                .AsNoTracking()
                .Where(c => c.ComicId == id)
                .Select(c => new ComicDto
                {
                    ComicId = c.ComicId,
                    Serie = c.Serie,
                    Number = c.Number,
                    Title = c.Title,
                    CreatedBy = c.CreatedBy,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt
                })
                .FirstOrDefaultAsync();

            return comic is null ? NotFound() : Ok(comic);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<ActionResult<ComicDto>> Create(CreateComicDto dto)
        {
            var comic = new Comic
            {
                Serie = dto.Serie,
                Number = dto.Number,
                Title = dto.Title,
                CreatedBy = dto.CreatedBy,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Comics.Add(comic);
            await _context.SaveChangesAsync();

            var result = new ComicDto
            {
                ComicId = comic.ComicId,
                Serie = comic.Serie,
                Number = comic.Number,
                Title = comic.Title,
                CreatedBy = comic.CreatedBy,
                CreatedAt = comic.CreatedAt,
                UpdatedAt = comic.UpdatedAt
            };

            return CreatedAtAction(nameof(GetById), new { id = comic.ComicId }, result);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<ActionResult<ComicDto>> Update(int id, UpdateComicDto dto)
        {
            var comic = await _context.Comics.FindAsync(id);
            if (comic is null)
            {
                return NotFound();
            }

            comic.Serie = dto.Serie;
            comic.Number = dto.Number;
            comic.Title = dto.Title;
            comic.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new ComicDto
            {
                ComicId = comic.ComicId,
                Serie = comic.Serie,
                Number = comic.Number,
                Title = comic.Title,
                CreatedBy = comic.CreatedBy,
                CreatedAt = comic.CreatedAt,
                UpdatedAt = comic.UpdatedAt
            });
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> Delete(int id)
        {
            var comic = await _context.Comics.FindAsync(id);
            if (comic is null)
            {
                return NotFound();
            }

            _context.Comics.Remove(comic);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
