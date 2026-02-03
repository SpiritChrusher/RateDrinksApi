using RateDrinksApi.Models;

namespace RateDrinksApi.Services;

using RateDrinksApi.Models.Dto;

using System.Threading.Tasks;
using System.Collections.Generic;

public interface IDrinksService
{
    Task<IReadOnlyList<AlcoholicDrink>> GetAllDrinksAsync(AlcoholType? type = null);
    Task<AlcoholicDrink?> GetDrinkByIdAsync(string id);
    Task<(IReadOnlyList<AlcoholicDrink> Added, IReadOnlyList<string> Errors)> AddDrinksAsync(IEnumerable<AlcoholicDrink> drinks);
    Task<(bool Success, bool NotFound, string? Error)> UpdateDrinkAsync(string id, AlcoholicDrink drink);
    Task<(bool Success, bool NotFound)> DeleteDrinkAsync(string id);
}