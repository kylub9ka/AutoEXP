using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoService.API.Models;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace AutoService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly User19DbContext _context;

        public OrdersController(User19DbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            try
            {
                var orders = await _context.KursZakazies
                    .Include(z => z.KodKlientaNavigation)
                    .Include(z => z.KodAvtoNavigation)
                    .Include(z => z.KodUslugiNavigation)
                    .Include(z => z.KodIspolnitelyaNavigation)
                    .Select(z => new
                    {
                        Kod_zakaza = z.KodZakaza,
                        Kod_klienta = z.KodKlienta,
                        Klient = z.KodKlientaNavigation.Fio,
                        Telefon = z.KodKlientaNavigation.Telefon,
                        Avto = z.KodAvtoNavigation.Marka + " (" + z.KodAvtoNavigation.Gosnomer + ")",
                        Usluga = string.IsNullOrEmpty(z.SpisokUslug) ? z.KodUslugiNavigation.Nazvanie : z.SpisokUslug,
                        Master = z.KodIspolnitelyaNavigation != null ? z.KodIspolnitelyaNavigation.Fio : "Не назначен",
                        MasterId = z.KodIspolnitelya,
                        Cena = z.ItogovayaStoimost,
                        Status = z.Status,
                        Data = z.DataZakaza
                    })
                    .ToListAsync();

                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message, details = ex.InnerException?.Message });
            }
        }

        [HttpPost("create-with-car")]
        public async Task<IActionResult> CreateWithCar([FromBody] CreateWithCarDto dto)
        {
            try
            {
                if (dto.UslugaIds == null || dto.UslugaIds.Count == 0)
                    return BadRequest("Выберите хотя бы одну услугу");

                var newCar = new KursAvtomobili
                {
                    KodKlienta = dto.KlientId,
                    Marka = dto.Marka,
                    Gosnomer = dto.Gosnomer
                };
                _context.KursAvtomobilis.Add(newCar);
                await _context.SaveChangesAsync();

                var uslugiNames = await _context.KursUslugis
                    .Where(u => dto.UslugaIds.Contains(u.KodUslugi))
                    .Select(u => u.Nazvanie)
                    .ToListAsync();

                string spisokStr = string.Join(", ", uslugiNames);

                decimal baseCost = dto.Cost * dto.UslugaIds.Count;
                decimal finalCost = baseCost;
                if (dto.Discount > 0) finalCost = finalCost - (finalCost * dto.Discount / 100);

                var newOrder = new KursZakazy
                {
                    KodKlienta = dto.KlientId,
                    KodAvto = newCar.KodAvto,
                    KodUslugi = dto.UslugaIds.First(),
                    SpisokUslug = spisokStr,
                    KodIspolnitelya = null,
                    ItogovayaStoimost = finalCost,
                    Status = "Ожидает",
                    DataZakaza = DateTime.Now
                };

                _context.KursZakazies.Add(newOrder);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Заказ успешно создан" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message, details = ex.InnerException?.Message });
            }
        }

        [HttpPut("{id}/take")]
        public async Task<IActionResult> TakeOrder(int id, [FromQuery] int masterId)
        {
            var order = await _context.KursZakazies.FindAsync(id);
            if (order == null) return NotFound();
            if (order.Status != "Ожидает") return BadRequest("Заказ уже взят");

            order.KodIspolnitelya = masterId;
            order.Status = "В работе";
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("{id}/complete")]
        public async Task<IActionResult> CompleteOrder(int id)
        {
            var order = await _context.KursZakazies.FindAsync(id);
            if (order == null) return NotFound();

            order.Status = "Выполнен";
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _context.KursZakazies.FindAsync(id);
            if (order == null) return NotFound();

            _context.KursZakazies.Remove(order);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }

    public class CreateWithCarDto
    {
        public int KlientId { get; set; }
        public string Marka { get; set; }
        public string Gosnomer { get; set; }
        public List<int> UslugaIds { get; set; }
        public decimal Cost { get; set; }
        public int Discount { get; set; }
    }
}