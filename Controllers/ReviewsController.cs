using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Backend.Models; 
using Backend.DTOs;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ReviewsController : ControllerBase
{
    private readonly ApplicationDbContext _context; 

    public ReviewsController(ApplicationDbContext context) => _context = context;

    [HttpGet("my")]
    public async Task<ActionResult<IEnumerable<ReviewDto>>> GetMyReviews()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var reviews = await _context.Reviews
            .Include(r => r.Hotel)
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new ReviewDto(
                r.Id,
                r.HotelId,
                r.Hotel != null ? r.Hotel.Name : "Unknown",
                r.Rating,
                r.Comment,
                r.CreatedAt
            ))
            .ToListAsync();

        return Ok(reviews);
    }

    [HttpPost]
    public async Task<IActionResult> CreateReview(CreateReviewDto dto)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var review = new Review
        {
            HotelId = dto.HotelId,
            UserId = userId,
            Rating = dto.Rating,
            Comment = dto.Comment,
            CreatedAt = DateTime.UtcNow
        };

        _context.Reviews.Add(review);
        await _context.SaveChangesAsync();

        return Ok();
    }
}
