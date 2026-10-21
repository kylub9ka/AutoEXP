using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoService.API.Models;
using System.Threading.Tasks;
using System.Linq;

namespace AutoService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MastersController : ControllerBase
    {
        private readonly User19DbContext _context;

        public MastersController(User19DbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetMasters()
        {
            var masters = await _context.KursIspolnitelis
                .Select(m => new { kod = m.KodIspolnitelya, fio = m.Fio, spec = m.Specializaciya })
                .ToListAsync();
            return Ok(masters);
        }

        [HttpPost]
        public async Task<IActionResult> AddMaster([FromBody] MasterDto dto)
        {
            var master = new KursIspolniteli { Fio = dto.Fio, Specializaciya = dto.Spec };
            _context.KursIspolnitelis.Add(master);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMaster(int id)
        {
            var master = await _context.KursIspolnitelis.FindAsync(id);
            if (master == null) return NotFound();

            var orders = await _context.KursZakazies.Where(z => z.KodIspolnitelya == id).ToListAsync();
            foreach (var o in orders) { o.KodIspolnitelya = null; }

            _context.KursIspolnitelis.Remove(master);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }

    public class MasterDto
    {
        public string Fio { get; set; }
        public string Spec { get; set; }
    }
}