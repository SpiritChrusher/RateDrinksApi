using System.Text.Json;
using RateDrinksApi.Models;

namespace RateDrinksApi.Utils;

public static class AlcoholicDrinkCosmosDeserializer
{
    public static AlcoholicDrink? Deserialize(string json)
    {
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;
        if (!root.TryGetProperty("Type", out var typeProp))
            return null;
        var typeString = typeProp.GetString();
        if (!Enum.TryParse<AlcoholType>(typeString, out var type))
            return null;
        return type switch
        {
            AlcoholType.Beer => JsonSerializer.Deserialize<Beer>(json),
            AlcoholType.Wine => JsonSerializer.Deserialize<Wine>(json),
            AlcoholType.Vodka => JsonSerializer.Deserialize<Vodka>(json),
            _ => JsonSerializer.Deserialize<AlcoholicDrink>(json)
        };
    }
}
