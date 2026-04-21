    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Backend.Models;

    namespace Backend.Controllers;

    [ApiController]
    [Route("api/[controller]")]
    public class FavoritesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public FavoritesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> AddToFavorite(Guid userId, int hotelId)
        {
            var exists = await _context.Favorites
                .AnyAsync(f => f.UserId == userId && f.HotelId == hotelId);

            if (exists) return BadRequest("Этот отель уже в избранном");

            var favorite = new Favorite
            {
                UserId = userId,
                HotelId = hotelId
            };

            _context.Favorites.Add(favorite);
            await _context.SaveChangesAsync();

            return Ok("Отель добавлен в избранное");
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetFavorites(Guid userId)
        {
            var favorites = await _context.Favorites
                .Where(f => f.UserId == userId)
                .Select(f => f.HotelId)
                .ToListAsync();

            return Ok(favorites);
        }

        [HttpDelete]
        public async Task<IActionResult> RemoveFromFavorite(Guid userId, int hotelId)
        {
            var favorite = await _context.Favorites
                .FirstOrDefaultAsync(f => f.UserId == userId && f.HotelId == hotelId);

            if (favorite == null) return NotFound();

            _context.Favorites.Remove(favorite);
            await _context.SaveChangesAsync();

            return Ok("Отель удален из избранного");
        }

        [HttpGet("{userId}/details")]
public async Task<IActionResult> GetFavoriteHotels(Guid userId)
{
    var favoriteHotels = await _context.Favorites
        .Where(f => f.UserId == userId)
        .Include(f => f.Hotel)
        .Select(f => f.Hotel)   
        .ToListAsync();

    return Ok(favoriteHotels);
}
    }