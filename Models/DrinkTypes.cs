namespace RateDrinksApi.Models
{
    public class Beer : AlcoholicDrink, IBeer
    {
        public string Brewery { get; set; } = string.Empty;
    }

    public class Wine : AlcoholicDrink, IWine
    {
        public string Vineyard { get; set; } = string.Empty;
        public int Year { get; set; }
    }

    public class Vodka : AlcoholicDrink, IVodka
    {
        public string Distillery { get; set; } = string.Empty;
    }
}