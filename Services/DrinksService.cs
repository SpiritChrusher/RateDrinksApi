using RateDrinksApi.Models;
using RateDrinksApi.Repositories;
using RateDrinksApi.Models.Dto;
using Microsoft.Extensions.Logging;

namespace RateDrinksApi.Services
{
    public class DrinksService : IDrinksService
    {
        private readonly IDrinksRepository _drinksRepository;
        private readonly ILogger<DrinksService> _logger;

        public DrinksService(IDrinksRepository drinksRepository, ILogger<DrinksService> logger)
        {
            _drinksRepository = drinksRepository;
            _logger = logger;
        }

        public async Task<IReadOnlyList<AlcoholicDrink>> GetAllDrinksAsync(AlcoholType? type = null)
        {
            _logger.LogInformation("Fetching all drinks. Type filter: {Type}", type);
            var result = await _drinksRepository.GetAllAsync(type);
            _logger.LogInformation("Fetched {Count} drinks.", result.Count);
            return result;
        }

        public async Task<AlcoholicDrink?> GetDrinkByIdAsync(string id)
        {
            _logger.LogInformation("Fetching drink by id: {Id}", id);
            var drink = await _drinksRepository.GetByIdAsync(id);
            if (drink == null)
                _logger.LogWarning("Drink not found: {Id}", id);
            else
                _logger.LogInformation("Drink found: {Id}", id);
            return drink;
        }

        public async Task<(IReadOnlyList<AlcoholicDrink> Added, IReadOnlyList<string> Errors)> AddDrinksAsync(IEnumerable<AlcoholicDrink> drinks)
        {
            var list = drinks?.ToList() ?? [];
            _logger.LogInformation("Validating {Count} drinks to add.", list.Count);
            var errors = Extensions.DrinkMappingExtensions.ValidateDrinks(list);
            if (errors.Count > 0)
            {
                _logger.LogWarning("Validation failed for {Count} drinks. Errors: {Errors}", list.Count, string.Join(", ", errors));
                return (Array.Empty<AlcoholicDrink>(), errors);
            }
            _logger.LogInformation("Adding {Count} drinks to repository.", list.Count);
            foreach (var drink in list)
            {
                await _drinksRepository.AddAsync(drink);
                _logger.LogInformation("Added drink: {DrinkName}", drink.Name);
            }
            return (list, Array.Empty<string>());
        }

        public async Task<(bool Success, bool NotFound, string? Error)> UpdateDrinkAsync(string id, AlcoholicDrink drink)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                _logger.LogWarning("Update failed: Invalid id.");
                return (false, false, "Invalid id.");
            }
            var error = Extensions.DrinkMappingExtensions.ValidateDrink(drink);
            if (!string.IsNullOrWhiteSpace(error))
            {
                _logger.LogWarning("Update failed for drink {Id}: {Error}", id, error);
                return (false, false, error);
            }
            _logger.LogInformation("Updating drink: {Id}", id);
            await _drinksRepository.UpdateAsync(drink);
            _logger.LogInformation("Drink updated: {Id}", id);
            return (true, false, null);
        }

        public async Task<(bool Success, bool NotFound)> DeleteDrinkAsync(string id)
        {
            _logger.LogInformation("Deleting drink: {Id}", id);
            var drink = await _drinksRepository.GetByIdAsync(id);
            if (drink == null)
            {
                _logger.LogWarning("Delete failed: Drink not found: {Id}", id);
                return (false, true);
            }
            await _drinksRepository.DeleteAsync(id);
            _logger.LogInformation("Drink deleted: {Id}", id);
            return (true, false);
        }
    }
}