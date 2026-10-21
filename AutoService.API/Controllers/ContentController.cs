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
    public class ContentController : ControllerBase
    {
        private readonly User19DbContext _context;

        public ContentController(User19DbContext context)
        {
            _context = context;
        }

        [HttpGet("promotions")]
        public async Task<IActionResult> GetPromotions()
        {
            var data = await _context.KursAkciis
                .Select(x => new { Id = x.KodAkcii, Title = x.Zagolovok, Desc = x.Opisanie, Discount = x.Skidka })
                .ToListAsync();
            return Ok(data);
        }

        [HttpGet("reviews")]
        public async Task<IActionResult> GetReviews()
        {
            var data = await _context.KursOtzvies
                .Include(o => o.KodKlientaNavigation)
                .Include(o => o.KodIspolnitelyaNavigation)
                .Select(o => new {
                    Id = o.KodOtzyva,
                    Klient = o.KodKlientaNavigation.Fio,
                    Master = o.KodIspolnitelyaNavigation.Fio,
                    Ocenka = o.Ocenka,
                    Text = o.TextOtzyva,
                    Data = o.DataOtzyva
                })
                .ToListAsync();
            return Ok(data);
        }

        [HttpPost("reviews")]
        public async Task<IActionResult> AddReview([FromBody] ReviewDto dto)
        {
            var review = new KursOtzvy
            {
                KodKlienta = dto.KlientId,
                KodIspolnitelya = dto.MasterId,
                Ocenka = dto.Ocenka,
                TextOtzyva = dto.Text,
                DataOtzyva = DateTime.Now
            };
            _context.KursOtzvies.Add(review);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }

    public class ReviewDto
    {
        public int KlientId { get; set; }
        public int MasterId { get; set; }
        public int Ocenka { get; set; }
        public string Text { get; set; }
    }
}