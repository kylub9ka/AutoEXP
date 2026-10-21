using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoService.API.Models;
using System.Threading.Tasks;
using System.Linq;

namespace AutoService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DictionaryController : ControllerBase
    {
        private readonly User19DbContext _context;

        public DictionaryController(User19DbContext context)
        {
            _context = context;
        }

        [HttpGet("clients")]
        public async Task<IActionResult> GetClients()
        {
            var data = await _context.KursKlienties.Select(x => new { Id = x.KodKlienta, Name = x.Fio }).ToListAsync();
            return Ok(data);
        }

        [HttpGet("cars")]
        public async Task<IActionResult> GetCars()
        {
            var data = await _context.KursAvtomobilis.Select(x => new { Id = x.KodAvto, Name = x.Marka + " (" + x.Gosnomer + ")" }).ToListAsync();
            return Ok(data);
        }

        [HttpGet("services")]
        public async Task<IActionResult> GetServices()
        {
            var data = await _context.KursUslugis.Select(x => new { Id = x.KodUslugi, Name = x.Nazvanie }).ToListAsync();
            return Ok(data);
        }

        [HttpGet("masters")]
        public async Task<IActionResult> GetMasters()
        {
            var data = await _context.KursIspolnitelis.Select(x => new { Id = x.KodIspolnitelya, Name = x.Fio }).ToListAsync();
            return Ok(data);
        }

        [HttpGet("clients-full")]
        public async Task<IActionResult> GetClientsFull()
        {
            var data = await _context.KursKlienties.Select(x => new { Kod = x.KodKlienta, Fio = x.Fio, Telefon = x.Telefon }).ToListAsync();
            return Ok(data);
        }
    }
}