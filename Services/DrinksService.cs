using RateDrinksApi.Models;
using RateDrinksApi.Repositories;

namespace RateDrinksApi.Services;


public class DrinksService : IDrinksService
{
    private readonly IDrinksRepository _drinksRepository;

    public DrinksService(IDrinksRepository drinksRepository)
    {
        _drinksRepository = drinksRepository;
    }


    public IReadOnlyList<AlcoholicDrink> GetAllDrinks(AlcoholType? type = null)
    {
        return _drinksRepository.GetAll(type).ToList();
    }


    public AlcoholicDrink? GetDrinkById(string id)
    {
        return _drinksRepository.GetById(id);
    }


    public (IReadOnlyList<AlcoholicDrink> Added, IReadOnlyList<string> Errors) AddDrinks(IEnumerable<AlcoholicDrink> drinks)
    {
        var list = drinks?.ToList() ?? [];
        var errors = ValidateDrinks(list);
        if (errors.Count > 0)
        {
            return (Array.Empty<AlcoholicDrink>(), errors);
        }
        foreach (var drink in list)
        {
            _drinksRepository.Add(drink);
        }
        return (list, Array.Empty<string>());
    }


    public (bool Success, bool NotFound, string? Error) UpdateDrink(string id, AlcoholicDrink drink)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return (false, false, "Invalid id.");
        }
        drink.Id = id;
        var error = ValidateDrink(drink);
        if (!string.IsNullOrWhiteSpace(error))
        {
            return (false, false, error);
        }
        _drinksRepository.Update(drink);
        // Cosmos DB upsert does not return affected rows, so assume success if no exception
        return (true, false, null);
    }


    public (bool Success, bool NotFound) DeleteDrink(string id)
    {
        var drink = _drinksRepository.GetById(id);
        if (drink == null)
        {
            return (false, true);
        }
        _drinksRepository.Delete(id);
        return (true, false);
    }

    private static List<string> ValidateDrinks(IEnumerable<AlcoholicDrink> drinks)
    {
        var errors = new List<string>();
        foreach (var drink in drinks)
        {
            var error = ValidateDrink(drink);
            if (!string.IsNullOrWhiteSpace(error))
            {
                errors.Add(string.IsNullOrWhiteSpace(drink.Name)
                    ? error
                    : $"{drink.Name}: {error}");
            }
        }
        return errors;
    }

    private static string? ValidateDrink(AlcoholicDrink drink)
    {
        if (drink is null)
        {
            return "Drink payload is required.";
        }

        if (string.IsNullOrWhiteSpace(drink.Name))
        {
            return "Name is required.";
        }

        if (drink.AlcoholContent < 0 || drink.AlcoholContent > 100)
        {
            return "AlcoholContent must be between 0 and 100.";
        }

        switch (drink.Type)
        {
            case AlcoholType.Beer:
                if (drink is not Beer beer)
                    return "Type is Beer but payload is not a Beer.";
                if (string.IsNullOrWhiteSpace(beer.Brewery))
                    return "Brewery is required for beer.";
                return null;

            case AlcoholType.Wine:
                if (drink is not Wine wine)
                    return "Type is Wine but payload is not a Wine.";
                if (string.IsNullOrWhiteSpace(wine.Vineyard))
                    return "Vineyard is required for wine.";
                var currentYear = DateTime.UtcNow.Year + 1;
                if (wine.Year < 1900 || wine.Year > currentYear)
                    return "Year must be between 1900 and next year.";
                return null;

            case AlcoholType.Vodka:
                if (drink is not Vodka vodka)
                    return "Type is Vodka but payload is not a Vodka.";
                if (string.IsNullOrWhiteSpace(vodka.Distillery))
                    return "Distillery is required for vodka.";
                return null;

            default:
                return "Unsupported drink type.";
        }
    }
}