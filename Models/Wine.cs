namespace RateDrinksApi.Models;

public enum WineType
{
    Red,
    White,
    Rose,
    Sparkling,
    Dessert,
    Fortified,
    Other
}

public class Wine : AlcoholicDrink
{
    public WineType WineType { get; set; }
    public string Vineyard { get; set; } = string.Empty;
    public int Year { get; set; }
}
