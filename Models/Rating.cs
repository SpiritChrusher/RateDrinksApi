using System.ComponentModel.DataAnnotations;

namespace RateDrinksApi.Models;

public class Rating
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int Score { get; set; } // e.g., 1-5 stars

    [Required]
    public string DrinkId { get; set; } = string.Empty; // Foreign key to AlcoholicDrink (Cosmos DB)

    [Required]
    public string UserId { get; set; } = string.Empty;

    public string Comment { get; set; } = string.Empty;
}
