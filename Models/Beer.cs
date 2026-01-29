namespace RateDrinksApi.Models;

public enum BeerType
{
    Lager,
    Ale,
    Stout,
    ImperialStout,
    IPA,
    ImperialIPA,
    PaleAle,
    Pilsner,
    Porter,
    Wheat,
    Other
}

public class Beer : AlcoholicDrink
{
    public BeerType BeerType { get; set; }
    public string Brewery { get; set; } = string.Empty;
}
