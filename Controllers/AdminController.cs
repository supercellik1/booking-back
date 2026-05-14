using Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _db;

        public AdminController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _db.Users
                .Select(u => new
                {
                    u.Id,
                    u.Email,
                    u.FullName,
                    u.Role
                })
                .ToListAsync();
            return Ok(users);
        }

        [HttpPost("users/{id}/make-admin")]
        public async Task<IActionResult> MakeAdmin(Guid id)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null) return NotFound();
            user.Role = "Admin";
            await _db.SaveChangesAsync();
            return Ok(new { message = "Пользователь стал админом" });
        }

        [HttpPost("users/{id}/remove-admin")]
        public async Task<IActionResult> RemoveAdmin(Guid id)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null) return NotFound();
            user.Role = "traveler";
            await _db.SaveChangesAsync();
            return Ok(new { message = "Админ разжалован в обычного пользователя" });
        }

        [HttpPost("users/{id}/make-manager")]
        public async Task<IActionResult> MakeManager(Guid id)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null) return NotFound();
            user.Role = "manager";
            await _db.SaveChangesAsync();
            return Ok(new { message = "Пользователь стал менеджером" });
        }

        [HttpPost("users/{id}/remove-manager")]
        public async Task<IActionResult> RemoveManager(Guid id)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null) return NotFound();
            user.Role = "traveler";
            await _db.SaveChangesAsync();
            return Ok(new { message = "Менеджер разжалован в обычного пользователя" });
        }
    }
}