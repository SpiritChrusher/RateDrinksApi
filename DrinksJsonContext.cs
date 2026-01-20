using System.Text.Json.Serialization;
using RateDrinksApi.Models;

namespace RateDrinksApi;

[JsonSerializable(typeof(Beer))]
[JsonSerializable(typeof(Wine))]
[JsonSerializable(typeof(Vodka))]
[JsonSerializable(typeof(AlcoholicDrink))]
[JsonSerializable(typeof(Rating))]
[JsonSerializable(typeof(AverageRatingResponse))]
// Add support for serializing collections
[JsonSerializable(typeof(IEnumerable<Beer>))]
[JsonSerializable(typeof(IEnumerable<Wine>))]
[JsonSerializable(typeof(IEnumerable<Vodka>))]
[JsonSerializable(typeof(IEnumerable<AlcoholicDrink>))]
[JsonSerializable(typeof(IEnumerable<Rating>))]
// Add support for serializing and deserializing List<T>
[JsonSerializable(typeof(List<Beer>))]
[JsonSerializable(typeof(List<Wine>))]
[JsonSerializable(typeof(List<Vodka>))]
[JsonSerializable(typeof(List<AlcoholicDrink>))]
[JsonSerializable(typeof(List<Rating>))]
//collections for JsonResult
[JsonSerializable(typeof(ICollection<Beer>))]
[JsonSerializable(typeof(ICollection<Wine>))]
[JsonSerializable(typeof(ICollection<Vodka>))]
[JsonSerializable(typeof(ICollection<AlcoholicDrink>))]
[JsonSerializable(typeof(ICollection<Rating>))]
//IEnumerable for JsonResult
[JsonSerializable(typeof(IEnumerable<Beer>))]
[JsonSerializable(typeof(IEnumerable<Wine>))]
[JsonSerializable(typeof(IEnumerable<Vodka>))]
[JsonSerializable(typeof(IEnumerable<AlcoholicDrink>))]
[JsonSerializable(typeof(IEnumerable<Rating>))]
public partial class DrinksJsonContext : JsonSerializerContext
{
}
