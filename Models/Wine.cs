using System.Text.Json.Serialization;

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
    [JsonPropertyName("wineType")]
    public WineType WineType { get; set; }
    [JsonPropertyName("vineyard")]
    public string Vineyard { get; set; } = string.Empty;
    [JsonPropertyName("year")]
    public int Year { get; set; }
    [JsonPropertyName("grape")]
    public string? Grape { get; set; }
    [JsonPropertyName("origin")]
    public string? Origin { get; set; }
    [JsonPropertyName("color")]
    public string? Color { get; set; }
    [JsonPropertyName("description")]
    public string? Description { get; set; }
}
