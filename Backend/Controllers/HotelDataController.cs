using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Models;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HotelDataController : ControllerBase
{
    private readonly AppDbContext _context;

    public HotelDataController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Hotel>>> GetHotels([FromQuery] string? country)
    {
        var query = _context.Hotels.AsQueryable();

        if (!string.IsNullOrEmpty(country))
        {
            query = query.Where(h => h.Country == country);
        }

        return await query.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Hotel>> GetHotel(int id)
    {
        var hotel = await _context.Hotels.FindAsync(id);

        if (hotel == null)
        {
            return NotFound();
        }

        return hotel;
    }
}
