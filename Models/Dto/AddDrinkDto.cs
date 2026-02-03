using System.ComponentModel.DataAnnotations;

namespace RateDrinksApi.Models.Dto
{
    public record AddDrinkDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Type { get; set; }
        [Required]
        public double AlcoholContent { get; set; }
        // Optional fields for specific types
        public double? Bitterness { get; set; } // Beer (IBU)
        public string? Ibu { get; set; } // Beer (string from frontend)
        public string? Brewery { get; set; } // Beer
        public string? Grape { get; set; } // Wine
        public string? Origin { get; set; } // Vodka, Wine, Whiskey
        public string? Color { get; set; } // Beer, Wine
        public string? Description { get; set; }
    }
}
