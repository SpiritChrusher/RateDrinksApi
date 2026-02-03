using System.Text.Json.Serialization;
using Newtonsoft.Json;

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

public class AlcoholicDrink
{
    [JsonProperty("id")]
    public string Id { get; set; } = default!;
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    [JsonPropertyName("alcoholContent")]
    public double AlcoholContent { get; set; } // Percentage
    [JsonPropertyName("type")]
    public AlcoholType Type { get; set; }
}