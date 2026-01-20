using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RateDrinksApi.Models;

[Table("Drinks")]
public class DrinkRecord
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Range(0, 100)]
    public double AlcoholContent { get; set; }

    [Required]
    public AlcoholType Type { get; set; }

    // Beer-specific
    public BeerType? BeerType { get; set; }
    public string? Brewery { get; set; }

    // Wine-specific
    public WineType? WineType { get; set; }
    public string? Vineyard { get; set; }
    public int? Year { get; set; }

    // Vodka-specific
    public VodkaType? VodkaType { get; set; }
    public string? Distillery { get; set; }
}

