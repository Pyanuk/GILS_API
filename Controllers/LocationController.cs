using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoundPlayerAPI.Models;

namespace SoundPlayerAPI.Controllers
{
    [Route("api/location")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LocationController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("countries")]
        public async Task<IActionResult> GetCountries()
        {
            var countries = await _context.Countries
                .Select(c => new
                {
                    IdCountry = c.IdCountry,
                    Countryname = c.Countryname
                })
                .ToListAsync();

            return Ok(countries);
        }

        [HttpGet("cities")]
        public async Task<IActionResult> GetCities()
        {
            var cities = await _context.Cities
                .Select(c => new
                {
                    IdCity = c.IdCity,
                    Cityname = c.Cityname,
                    CountryId = c.Countryid
                })
                .ToListAsync();

            return Ok(cities);
        }
    }
}
