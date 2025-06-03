using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoundPlayerAPI.Models;
using SoundPlayerAPI.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SoundPlayerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchArtistsController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public SearchArtistsController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET: api/SearchArtists?query=The+Dare
        [HttpGet]
        public async Task<ActionResult<ArtistSimpleDto>> SearchArtist([FromQuery] string query)
        {
            try
            {
                if (string.IsNullOrEmpty(query))
                    return BadRequest("Search query cannot be empty");

                string search = NormalizeSearchText(query);
                var artists = await _dbContext.Artists
                    .ToListAsync(); // Убраны Include, так как нужны только Id и Name

                foreach (var artist in artists)
                {
                    if (IsArtistMatch(search, artist))
                    {
                        var artistDto = new ArtistSimpleDto
                        {
                            ArtistId = artist.ArtistId,
                            Name = artist.Name
                        };
                        return Ok(artistDto);
                    }
                }

                return NotFound("Artist not found");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error searching artist: {ex.Message}");
            }
        }

        private string NormalizeSearchText(string input)
        {
            return input?.ToLower().Trim() ?? string.Empty;
        }

        private bool IsArtistMatch(string searchText, Artist artist)
        {
            if (artist == null || string.IsNullOrEmpty(searchText))
                return false;

            string artistName = NormalizeSearchText(artist.Name);
            string search = NormalizeSearchText(searchText);

            // Точное совпадение
            if (artistName == search)
                return true;

            // Вариации для "The Dare"
            var variations = new List<string>
            {
                "the dare",
                "зэ дар",
                "зе дар",
                "dare",
                "harrison patrick",
                "патрик харрисон",
                "patrick harrison"
            };

            // Проверяем, является ли имя артиста "The Dare" (в любом регистре)
            if (artistName.Contains("the dare") || artistName.Contains("dare"))
            {
                return variations.Contains(search);
            }

            // Проверяем перестановку имени и фамилии
            string[] nameParts = artistName.Split(' ');
            if (nameParts.Length >= 2)
            {
                string reversedName = $"{nameParts[1]} {nameParts[0]}".ToLower();
                if (search == reversedName)
                    return true;
            }

            return false;
        }

        // GET: api/SearchArtists/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ArtistSimpleDto>> GetArtistById(Guid id)
        {
            try
            {
                var artist = await _dbContext.Artists
                    .FirstOrDefaultAsync(a => a.ArtistId == id);

                if (artist == null)
                    return NotFound("Artist not found");

                var artistDto = new ArtistSimpleDto
                {
                    ArtistId = artist.ArtistId,
                    Name = artist.Name
                };
                return Ok(artistDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error loading artist: {ex.Message}");
            }
        }
    }
}
