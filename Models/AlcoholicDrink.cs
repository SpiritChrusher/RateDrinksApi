namespace RateDrinksApi.Models;

public enum AlcoholType
{
    Beer,
    Wine,
    Vodka,
    Gin,
    Whiskey,
    Rum,
    Tequila,
    Brandy,
    Other
}

public abstract class AlcoholicDrink
{
    public string? Id { get; set; } // CosmosDB id (e.g., Name + AlcoholContent)
    public string Name { get; set; } = string.Empty;
    public double AlcoholContent { get; set; } // Percentage
    public AlcoholType Type { get; set; }
}