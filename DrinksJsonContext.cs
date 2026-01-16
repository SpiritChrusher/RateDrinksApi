using System.Text.Json.Serialization;
using RateDrinksApi.Models;

namespace RateDrinksApi;

[JsonSerializable(typeof(Beer))]
[JsonSerializable(typeof(Wine))]
[JsonSerializable(typeof(Vodka))]
[JsonSerializable(typeof(AlcoholicDrink))]
// Add support for serializing collections
[JsonSerializable(typeof(IEnumerable<Beer>))]
[JsonSerializable(typeof(IEnumerable<Wine>))]
[JsonSerializable(typeof(IEnumerable<Vodka>))]
// Add support for serializing and deserializing List<T>
[JsonSerializable(typeof(List<Beer>))]
[JsonSerializable(typeof(List<Wine>))]
[JsonSerializable(typeof(List<Vodka>))]
//collections for JsonResult
[JsonSerializable(typeof(ICollection<Beer>))]
[JsonSerializable(typeof(ICollection<Wine>))]
[JsonSerializable(typeof(ICollection<Vodka>))]
//IEnumerable for JsonResult
[JsonSerializable(typeof(IEnumerable<Beer>))]
[JsonSerializable(typeof(IEnumerable<Wine>))]
[JsonSerializable(typeof(IEnumerable<Vodka>))]
public partial class DrinksJsonContext : JsonSerializerContext
{
}
