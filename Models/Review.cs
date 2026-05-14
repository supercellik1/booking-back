using System.ComponentModel.DataAnnotations;

namespace Backend.Models;

public class Review
{
    public int Id { get; set; }
    
    [Range(1, 5)]
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int HotelId { get; set; }
    public Hotel? Hotel { get; set; }

    public Guid UserId { get; set; }
    public User? User { get; set; }
}
