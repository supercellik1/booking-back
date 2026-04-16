namespace Backend.Models;

public class Hotel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public double Rating { get; set; }
    public string Description { get; set; } = string.Empty; 
    public string FullDescription { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Price { get; set; } = string.Empty;
    public string MapUrl { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty; 
    public List<string> Images { get; set; } = new();
}
