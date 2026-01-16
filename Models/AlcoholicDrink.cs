namespace RateDrinksApi.Models
{

    public abstract class AlcoholicDrink : IAlcoholicDrink
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public double AlcoholContent { get; set; } // Percentage
        public string Type { get; set; } = string.Empty;
    }
}