namespace Backend.DTOs;

public class HotelCreateDto
{
    public string Name { get; set; }
    public double Rating { get; set; }
    public string Description { get; set; }
    public string FullDescription { get; set; }
    public string Location { get; set; }
    public string Price { get; set; }
    public string MapUrl { get; set; }
    public string Country { get; set; }
    public List<string> Images { get; set; }
}