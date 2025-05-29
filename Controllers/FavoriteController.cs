using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoundPlayerAPI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SoundPlayerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FavoriteController : ControllerBase
    {
        private readonly AppDbContext _context;

        public FavoriteController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/Favorite
        [HttpPost]
        public async Task<IActionResult> AddFavoriteTrack([FromBody] FavoriteTrackInput favoriteInput)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var trackExists = await _context.Tracks.AnyAsync(t => t.TrackId == favoriteInput.TrackId);
            if (!trackExists)
            {
                return NotFound(new { message = "Трек не найден" });
            }

            var userExists = await _context.Users.AnyAsync(u => u.IdUser == favoriteInput.UserId);
            if (!userExists)
            {
                return NotFound(new { message = "Пользователь не найден" });
            }

            var alreadyFavorited = await _context.FavoriteTracks
                .AnyAsync(f => f.UserId == favoriteInput.UserId && f.TrackId == favoriteInput.TrackId);
            if (alreadyFavorited)
            {
                return Conflict(new { message = "Трек уже в избранном" });
            }

            var favoriteTrack = new FavoriteTrack
            {
                UserId = favoriteInput.UserId,
                TrackId = favoriteInput.TrackId
            };

            _context.FavoriteTracks.Add(favoriteTrack);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Трек добавлен в избранное", userId = favoriteTrack.UserId, trackId = favoriteTrack.TrackId, idFavorite = favoriteTrack.IdFavorite });
        }

        // GET: api/Favorite/{userId}
        [HttpGet("{userId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetFavoriteTracks(int userId)
        {
            var userExists = await _context.Users.AnyAsync(u => u.IdUser == userId);
            if (!userExists)
            {
                return NotFound(new { message = "Пользователь не найден" });
            }

            var favoriteTracks = await _context.FavoriteTracks
                .Where(f => f.UserId == userId)
                .Include(f => f.Track)
                    .ThenInclude(t => t.Artists) // Связь с Artist
                .Select(f => new
                {
                    f.IdFavorite,
                    f.UserId,
                    f.TrackId,
                    f.Track.Title,
                    ArtistName = f.Track.Artists.FirstOrDefault().Name ?? "Неизвестный исполнитель" // Берем первого исполнителя
                })
                .ToListAsync();

            return Ok(favoriteTracks);
        }
    }

    public class FavoriteTrackInput
    {
        public int UserId { get; set; }
        public Guid TrackId { get; set; }
    }
}