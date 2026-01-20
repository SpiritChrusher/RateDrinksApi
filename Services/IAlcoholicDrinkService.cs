using RateDrinksApi.Models;
using RateDrinksApi.Repositories;

namespace RateDrinksApi.Services
{
    public interface IAlcoholicDrinkService
    {
        IReadOnlyList<AlcoholicDrink> GetAllDrinks();
        AlcoholicDrink? GetDrinkById(int id);
        (IReadOnlyList<AlcoholicDrink> Added, IReadOnlyList<string> Errors) AddDrinks(IEnumerable<AlcoholicDrink> drinks);
        (bool Success, bool NotFound, string? Error) UpdateDrink(int id, AlcoholicDrink drink);
        (bool Success, bool NotFound) DeleteDrink(int id);
    }
}