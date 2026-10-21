using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoService.API.Models;
using System.Threading.Tasks;
using System.Linq;

namespace AutoService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminContentController : ControllerBase
    {
        private readonly User19DbContext _context;
        public AdminContentController(User19DbContext context) { _context = context; }

        [HttpPost("promotions")]
        public async Task<IActionResult> AddPromotion([FromBody] PromoDto dto)
        {
            var promo = new KursAkcii { Zagolovok = dto.Title, Opisanie = dto.Desc, Skidka = dto.Discount };
            _context.KursAkciis.Add(promo);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("promotions/{id}")]
        public async Task<IActionResult> DeletePromotion(int id)
        {
            var p = await _context.KursAkciis.FindAsync(id);
            if (p == null) return NotFound();
            _context.KursAkciis.Remove(p);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
    public class PromoDto { public string Title { get; set; } public string Desc { get; set; } public int Discount { get; set; } }
}