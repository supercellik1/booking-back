using Backend.DTOs;
using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly TokenService _tokenService;

    public AuthController(AppDbContext db, TokenService tokenService)
    {
        _db = db;
        _tokenService = tokenService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        if (await _db.Users.AnyAsync(u => u.Email == request.Email))
            return BadRequest("Email уже занят");

        var user = new User
        {
            Email = request.Email,
            FullName = request.FullName,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.PasswordHash) // Хешируем!
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return Ok(new { message = "Регистрация успешна" });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.PasswordHash, user.PasswordHash))
            return Unauthorized("Неверный email или пароль");

        var token = _tokenService.CreateToken(user);

        return Ok(new AuthResponse(
            token, 
            new UserDto(user.Id, user.Email, user.FullName)
        ));
    }
    
    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
    
        if (userIdClaim == null) 
        {
            return Unauthorized(new { message = "Пользователь не идентифицирован" });
        }

        var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;

        User user = null;

        if (!string.IsNullOrEmpty(userEmail))
        {   
            user = await _db.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
        }
        if (user == null && int.TryParse(userIdClaim.Value, out int numericId))
        {
            user = await _db.Users.FindAsync(numericId);
        }
        if (user == null) 
        {
            return NotFound(new { message = "Пользователь не найден в базе" });
        }
        
        return Ok(new {
            Id = user.Id,
            Email = user.Email,
            FullName = user.FullName
        });
    }
}
