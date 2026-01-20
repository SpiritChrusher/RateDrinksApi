using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RateDrinksApi.Models
{
    public class Rating
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int Score { get; set; } // e.g., 1-5 stars

        [Required]
        public int DrinkId { get; set; } // Foreign key to DrinkRecord

        [ForeignKey("DrinkId")]
        public DrinkRecord Drink { get; set; }

        // Optionally, add UserId or other fields for future expansion
    }
}
