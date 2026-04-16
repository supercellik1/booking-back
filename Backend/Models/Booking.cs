using System.ComponentModel.DataAnnotations;

namespace Backend.Models;

public class Booking
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid UserId { get; set; }
    public User User { get; set; } = null!; 

    [Required]
    public string HotelId { get; set; } = string.Empty;

    [Required]
    public string FullName { get; set; } = string.Empty;

    [Required]
    public string Email { get; set; } = string.Empty;

    [Required]
    public DateTime CheckIn { get; set; }

    [Required]
    public DateTime CheckOut { get; set; }

    public int Guests { get; set; } = 1;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
