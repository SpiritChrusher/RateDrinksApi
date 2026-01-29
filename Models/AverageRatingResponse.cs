namespace RateDrinksApi.Models;

public class AverageRatingResponse
{
    public string DrinkId { get; set; } = string.Empty;
    public double Average { get; set; }
}
