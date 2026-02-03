
using RateDrinksApi.Models;
using RateDrinksApi.Repositories;
using RateDrinksApi.Models.Dto;

namespace RateDrinksApi.Services
{
    public class DrinksService : IDrinksService
    {
        private readonly IDrinksRepository _drinksRepository;

        public DrinksService(IDrinksRepository drinksRepository)
        {
            _drinksRepository = drinksRepository;
        }


        public async Task<IReadOnlyList<AlcoholicDrink>> GetAllDrinksAsync(AlcoholType? type = null)
        {
            return await _drinksRepository.GetAllAsync(type);
        }

        public async Task<AlcoholicDrink?> GetDrinkByIdAsync(string id)
        {
            return await _drinksRepository.GetByIdAsync(id);
        }

        public async Task<(IReadOnlyList<AlcoholicDrink> Added, IReadOnlyList<string> Errors)> AddDrinksAsync(IEnumerable<AlcoholicDrink> drinks)
        {
            var list = drinks?.ToList() ?? [];
            System.Console.WriteLine($"Validating {list.Count} drinks to add.");
            var errors = Extensions.DrinkMappingExtensions.ValidateDrinks(list);
            if (errors.Count > 0)
            {
                return (Array.Empty<AlcoholicDrink>(), errors);
            }
            System.Console.WriteLine($"Adding {list.Count} drinks to repository.");
            foreach (var drink in list)
            {
                await _drinksRepository.AddAsync(drink);
            }
            return (list, Array.Empty<string>());
        }

        public async Task<(bool Success, bool NotFound, string? Error)> UpdateDrinkAsync(string id, AlcoholicDrink drink)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return (false, false, "Invalid id.");
            }
            var error = Extensions.DrinkMappingExtensions.ValidateDrink(drink);
            if (!string.IsNullOrWhiteSpace(error))
            {
                return (false, false, error);
            }
            await _drinksRepository.UpdateAsync(drink);//we'll need to pass down the id as well
            // Cosmos DB upsert does not return affected rows, so assume success if no exception
            return (true, false, null);
        }

        public async Task<(bool Success, bool NotFound)> DeleteDrinkAsync(string id)
        {
            var drink = await _drinksRepository.GetByIdAsync(id);
            if (drink == null)
            {
                return (false, true);
            }
            await _drinksRepository.DeleteAsync(id);
            return (true, false);
        }
    }
}