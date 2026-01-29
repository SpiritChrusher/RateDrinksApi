namespace RateDrinksApi.Models;

public enum VodkaType
{
    Plain,
    Flavored,
    Premium,
    Other
}

public class Vodka : AlcoholicDrink
{
    public VodkaType VodkaType { get; set; }
    public string Distillery { get; set; } = string.Empty;
}
