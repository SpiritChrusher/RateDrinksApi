using System.Text.Json.Serialization;

namespace RateDrinksApi.Models;

public class Whiskey : AlcoholicDrink
{
    [JsonPropertyName("origin")]
    public string? Origin { get; set; }
    [JsonPropertyName("description")]
    public string? Description { get; set; }
    // Add more whiskey-specific fields as needed
}
