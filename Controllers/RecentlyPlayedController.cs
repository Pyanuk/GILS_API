using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoundPlayerAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SoundPlayerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecentlyPlayedController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RecentlyPlayedController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/RecentlyPlayed
        [HttpPost]
        public async Task<IActionResult> AddRecentlyPlayed([FromBody] RecentlyPlayedInput input)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userExists = await _context.Users.AnyAsync(u => u.IdUser == input.UserId);
            if (!userExists)
                return NotFound(new { message = "Пользователь не найден" });

            var trackExists = await _context.Tracks.AnyAsync(t => t.TrackId == input.TrackId);
            if (!trackExists)
                return NotFound(new { message = "Трек не найден" });

            var newEntry = new RecentlyPlayed
            {
                UserId = input.UserId,
                TrackId = input.TrackId
            };

            _context.RecentlyPlayed.Add(newEntry);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Трек добавлен в недавно прослушанные" });
        }

        // GET: api/RecentlyPlayed/{userId}
        [HttpGet("{userId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetRecentlyPlayed(int userId)
        {
            var userExists = await _context.Users.AnyAsync(u => u.IdUser == userId);
            if (!userExists)
                return NotFound(new { message = "Пользователь не найден" });

            var recentTracks = await _context.RecentlyPlayed
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.RecentlyId)
                .Include(r => r.Track)
                .Select(r => new
                {
                    r.RecentlyId,
                    r.UserId,
                    r.TrackId,
                    r.Track.Title,
                    r.Track.FilePath,
                    r.Track.CoverPath
                })
                .ToListAsync();

            return Ok(recentTracks);
        }
    }

    // Входная модель
    public class RecentlyPlayedInput
    {
        public int UserId { get; set; }
        public Guid TrackId { get; set; }
    }
}
