using System.Text.Json.Serialization;

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
    [JsonPropertyName("beerType")]
    public BeerType BeerType { get; set; }
    [JsonPropertyName("brewery")]
    public string Brewery { get; set; } = string.Empty;
    [JsonPropertyName("bitterness")]
    public double Bitterness { get; set; }
    [JsonPropertyName("color")]
    public string? Color { get; set; }
    [JsonPropertyName("description")]
    public string? Description { get; set; }
}
