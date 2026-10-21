using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoService.API.Models;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace AutoService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly User19DbContext _context;

        public AuthController(User19DbContext context)
        {
            _context = context;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            try
            {
                var user = await _context.KursUsers
                    .FirstOrDefaultAsync(u => u.Login == dto.Login && u.Password == dto.Password);

                if (user == null)
                    return Unauthorized(new { message = "Неверный логин или пароль" });

                return Ok(new
                {
                    role = user.Role,
                    login = user.Login,
                    clientId = user.KodKlienta,
                    masterId = user.KodIspolnitelya
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Ошибка сервера", details = ex.Message });
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            try
            {
                if (await _context.KursUsers.AnyAsync(u => u.Login == dto.Login))
                    return BadRequest(new { message = "Логин уже занят" });

                var newClient = new KursKlienty { Fio = dto.Fio, Telefon = dto.Phone };
                _context.KursKlienties.Add(newClient);
                await _context.SaveChangesAsync();

                var newUser = new KursUser
                {
                    Login = dto.Login,
                    Password = dto.Password,
                    Role = "Client",
                    KodKlienta = newClient.KodKlienta
                };
                _context.KursUsers.Add(newUser);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Регистрация успешна" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Ошибка регистрации", details = ex.Message });
            }
        }
    }

    public class LoginDto
    {
        public string Login { get; set; }
        public string Password { get; set; }
    }

    public class RegisterDto
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public string Fio { get; set; }
        public string Phone { get; set; }
    }
}