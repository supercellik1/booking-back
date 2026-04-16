using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Backend.Models;
using Backend.Dtos;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers;

[ApiController]
[Route("api/HotelData/bookings")]
[Authorize] 
public class BookingsController : ControllerBase
{
    private readonly AppDbContext _context;

    public BookingsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> CreateBooking([FromBody] CreateBookingDto dto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (string.IsNullOrEmpty(userIdClaim))
            return Unauthorized("Пользователь не определен");

        var booking = new Booking
        {
            UserId = Guid.Parse(userIdClaim),
            HotelId = dto.HotelId,
            FullName = dto.FullName,
            Email = dto.Email,
            CheckIn = dto.CheckIn,
            CheckOut = dto.CheckOut,
            Guests = dto.Guests
        };
        
        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Бронирование успешно создано", bookingId = booking.Id });
    }


    [HttpGet("my")]
    public async Task<IActionResult> GetMyBookings()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();

        var userId = Guid.Parse(userIdClaim);

        var myBookings = await _context.Bookings
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.CreatedAt)
            .Select(b => new {
                b.Id,
                b.HotelId,
                b.CheckIn,
                b.CheckOut,
                b.Guests,
                HotelData = _context.Hotels
                    .Where(h => h.Id.ToString() == b.HotelId) 
                    .Select(h => new { h.Name, h.Rating, h.Location })
                    .FirstOrDefault()
            })
            .ToListAsync();

        var result = myBookings.Select(b => new {
            b.Id,
            b.HotelId,
            b.CheckIn,
            b.CheckOut,
            b.Guests,
            HotelName = b.HotelData?.Name ?? "Отель не найден",
            Rating = b.HotelData?.Rating ?? 0,
            Location = b.HotelData?.Location ?? "Локация не указана"
        });

        return Ok(result);
    }
}
