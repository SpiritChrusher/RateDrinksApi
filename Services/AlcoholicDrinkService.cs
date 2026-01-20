using Microsoft.Data.SqlClient;
using RateDrinksApi.Data;
using Microsoft.Extensions.Options;
using RateDrinksApi.Models;
using RateDrinksApi.Options;

namespace RateDrinksApi.Services
{
    public class AlcoholicDrinkService : IAlcoholicDrinkService
    {
        private readonly DrinksDbContext _db;

        public AlcoholicDrinkService(DrinksDbContext db)
        {
            _db = db;
        }

        public IReadOnlyList<AlcoholicDrink> GetAllDrinks()
        {
            var records = _db.Drinks.ToList();
            return records
                .Select(FromRecord)
                .Where(d => d is not null)
                .Cast<AlcoholicDrink>()
                .ToList();
        }

        public AlcoholicDrink? GetDrinkById(int id)
        {
            var record = _db.Drinks.Find(id);
            return record is null ? null : FromRecord(record);
        }

        public (IReadOnlyList<AlcoholicDrink> Added, IReadOnlyList<string> Errors) AddDrinks(IEnumerable<AlcoholicDrink> drinks)
        {
            var list = drinks?.ToList() ?? new List<AlcoholicDrink>();
            var errors = ValidateDrinks(list);
            if (errors.Count > 0)
            {
                return (Array.Empty<AlcoholicDrink>(), errors);
            }

            var records = list.Select(ToRecord).ToList();
            foreach (var record in records)
            {
                _db.Drinks.Add(record);
            }
            _db.SaveChanges();
            for (var i = 0; i < list.Count; i++)
            {
                list[i].Id = records[i].Id;
            }
            return (list, Array.Empty<string>());
        }

        public (bool Success, bool NotFound, string? Error) UpdateDrink(int id, AlcoholicDrink drink)
        {
            if (id <= 0)
            {
                return (false, false, "Invalid id.");
            }

            drink.Id = id;
            var error = ValidateDrink(drink);
            if (!string.IsNullOrWhiteSpace(error))
            {
                return (false, false, error);
            }

            var record = ToRecord(drink);
            record.Id = id;
            _db.Drinks.Update(record);
            var affected = _db.SaveChanges();
            return affected == 0 ? (false, true, null) : (true, false, null);
        }

        public (bool Success, bool NotFound) DeleteDrink(int id)
        {
            var record = _db.Drinks.Find(id);
            if (record == null)
            {
                return (false, true);
            }
            _db.Drinks.Remove(record);
            var affected = _db.SaveChanges();
            return affected == 0 ? (false, true) : (true, false);
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

        private static DrinkRecord ToRecord(AlcoholicDrink drink)
        {
            var record = new DrinkRecord
            {
                Id = drink.Id,
                Name = drink.Name.Trim(),
                AlcoholContent = drink.AlcoholContent,
                Type = drink.Type
            };

            ApplyToRecord(drink, record);
            return record;
        }

        private static void ApplyToRecord(AlcoholicDrink drink, DrinkRecord record)
        {
            record.Name = drink.Name.Trim();
            record.AlcoholContent = drink.AlcoholContent;
            record.Type = drink.Type;

            record.BeerType = null;
            record.Brewery = null;
            record.WineType = null;
            record.Vineyard = null;
            record.Year = null;
            record.VodkaType = null;
            record.Distillery = null;

            switch (drink)
            {
                case Beer beer:
                    record.BeerType = beer.BeerType;
                    record.Brewery = beer.Brewery?.Trim();
                    break;
                case Wine wine:
                    record.WineType = wine.WineType;
                    record.Vineyard = wine.Vineyard?.Trim();
                    record.Year = wine.Year;
                    break;
                case Vodka vodka:
                    record.VodkaType = vodka.VodkaType;
                    record.Distillery = vodka.Distillery?.Trim();
                    break;
            }
        }

        public static AlcoholicDrink? FromRecord(DrinkRecord record)
        {
            return record.Type switch
            {
                AlcoholType.Beer => new Beer
                {
                    Id = record.Id,
                    Name = record.Name,
                    AlcoholContent = record.AlcoholContent,
                    Type = record.Type,
                    BeerType = record.BeerType ?? BeerType.Other,
                    Brewery = record.Brewery ?? string.Empty
                },
                AlcoholType.Wine => new Wine
                {
                    Id = record.Id,
                    Name = record.Name,
                    AlcoholContent = record.AlcoholContent,
                    Type = record.Type,
                    WineType = record.WineType ?? WineType.Other,
                    Vineyard = record.Vineyard ?? string.Empty,
                    Year = record.Year ?? 0
                },
                AlcoholType.Vodka => new Vodka
                {
                    Id = record.Id,
                    Name = record.Name,
                    AlcoholContent = record.AlcoholContent,
                    Type = record.Type,
                    VodkaType = record.VodkaType ?? VodkaType.Other,
                    Distillery = record.Distillery ?? string.Empty
                },
                _ => null
            };
        }

        private static string ValidateTableName(string name, string fallback)
        {
            var value = string.IsNullOrWhiteSpace(name) ? fallback : name;
            foreach (var ch in value)
            {
                if (!char.IsLetterOrDigit(ch) && ch != '_')
                {
                    throw new InvalidOperationException("Drinks table name contains invalid characters.");
                }
            }

            return value;
        }
    }
}