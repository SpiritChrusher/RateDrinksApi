using System.Text.Json.Serialization;

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
    [JsonPropertyName("vodkaType")]
    public VodkaType VodkaType { get; set; }
    [JsonPropertyName("distillery")]
    public string Distillery { get; set; } = string.Empty;
    [JsonPropertyName("origin")]
    public string? Origin { get; set; }
    [JsonPropertyName("description")]
    public string? Description { get; set; }
}
