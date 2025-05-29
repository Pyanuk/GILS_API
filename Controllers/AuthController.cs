using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoundPlayerAPI.Models;
using SoundPlayerAPI.Models.DTOs;
using SoundPlayerAPI.Service;
using System.Threading.Tasks;

namespace SoundPlayerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            if (await _context.Users.AnyAsync(u => u.Email == model.Email))
                return BadRequest("Пользователь с таким email уже существует.");

            var user = new User
            {
                Firstname = model.Firstname,
                Lastname = model.Lastname,
                Email = model.Email,
                Phone = model.Phone,
                Birthday = model.Birthday,
                Cityid = model.Cityid,
                Countryid = model.Countryid,
                Roleid = 1, // по умолчанию "пользователь"
                PasswordUsers = PasswordHasher.HashPassword(model.Password)
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok("Регистрация прошла успешно.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (user == null || user.PasswordUsers == null || !PasswordHasher.VerifyPassword(model.Password, user.PasswordUsers))
                return Unauthorized("Неверный email или пароль.");

            return Ok(new
            {
                message = "Авторизация успешна",
                userId = user.IdUser,
                userName = user.Firstname + " " + user.Lastname,
                roleId = user.Roleid
                // можно добавить генерацию JWT при необходимости
            });
        }
    }
}
