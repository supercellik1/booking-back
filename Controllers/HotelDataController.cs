using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Backend.DTOs;

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

    [Authorize(Roles = "manager,Manager,Admin,admin")]
[HttpPost]
public async Task<IActionResult> CreateHotel([FromBody] HotelCreateDto dto)
{
    var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
    if (userIdClaim == null)
        return Unauthorized("User id not found in token.");

    var userId = Guid.Parse(userIdClaim.Value);

    var hotel = new Hotel
    {
        Name = dto.Name,
        Rating = dto.Rating,
        Description = dto.Description,
        FullDescription = dto.FullDescription,
        Location = dto.Location,
        Price = dto.Price,
        MapUrl = dto.MapUrl,
        Country = dto.Country,
        Images = dto.Images ?? new List<string>(),
        OwnerId = userId,
        Status = "Pending"
    };

    _context.Hotels.Add(hotel);
    await _context.SaveChangesAsync();

    return Ok(new { hotel.Id });
}

[Authorize(Roles = "Admin")]
[HttpPost("{id}/approve")]
[Authorize(Roles = "Admin")]
[HttpPost("{id}/approve-by-moderator")]
public async Task<IActionResult> ApproveHotel(int id)
{
    var hotel = await _context.Hotels.Include(h => h.Owner).FirstOrDefaultAsync(h => h.Id == id);
    if (hotel == null)
        return NotFound("Отель не найден.");

    if (hotel.Status == "Approved")
        return BadRequest("Отель уже одобрен.");

    if (hotel.Owner == null)
        return BadRequest("У отеля нет владельца.");

    hotel.Status = "Approved";
    hotel.Owner.Role = "manager";
    await _context.SaveChangesAsync();

    return Ok(new { message = "Отель одобрен, роль менеджера выдана." });
}
}
