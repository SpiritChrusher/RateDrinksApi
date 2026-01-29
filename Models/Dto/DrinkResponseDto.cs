namespace RateDrinksApi.Models.Dto;

public class DrinkResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public double AlcoholContent { get; set; }
    public string Type { get; set; } = string.Empty; // e.g. "Beer", "Wine", "Vodka"
    // Add specific properties for each drink type as needed
    // Example for Beer:
    public string? BeerType { get; set; }
    public string? Brewery { get; set; }
    // Example for Wine:
    public string? WineType { get; set; }
    public string? Vineyard { get; set; }
    public int? Year { get; set; }
    // Example for Vodka:
    public string? VodkaType { get; set; }
    public string? Distillery { get; set; }
}
