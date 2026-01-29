using RateDrinksApi.Models;

namespace RateDrinksApi.Services;

public interface IDrinksService
{
    IReadOnlyList<AlcoholicDrink> GetAllDrinks(AlcoholType? type = null);
    AlcoholicDrink? GetDrinkById(string id);
    (IReadOnlyList<AlcoholicDrink> Added, IReadOnlyList<string> Errors) AddDrinks(IEnumerable<AlcoholicDrink> drinks);
    (bool Success, bool NotFound, string? Error) UpdateDrink(string id, AlcoholicDrink drink);
    (bool Success, bool NotFound) DeleteDrink(string id);
}