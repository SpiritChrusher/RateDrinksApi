using System.Collections.Generic;
using RateDrinksApi.Models;
using RateDrinksApi.Models.Dto;

namespace RateDrinksApi.Extensions
{
    public static class DrinkMappingExtensions
    {
        public static (List<AlcoholicDrink> Drinks, List<string> Errors) ToAlcoholicDrinks(this IEnumerable<AddDrinkDto> dtos)
        {
            var drinks = new List<AlcoholicDrink>();
            var errors = new List<string>();
            foreach (var dto in dtos)
            {
                switch (dto.Type)
                {
                    case "Beer":
                        drinks.Add(new Beer
                        {
                            Name = dto.Name,
                            AlcoholContent = dto.AlcoholContent,
                            Bitterness = dto.Bitterness ?? (double.TryParse(dto.Ibu, out var ibuVal) ? ibuVal : 0),
                            Brewery = dto.Brewery ?? string.Empty,
                            Color = dto.Color,
                            Description = dto.Description,
                            Type = AlcoholType.Beer
                        });
                        break;
                    case "Wine":
                        drinks.Add(new Wine
                        {
                            Name = dto.Name,
                            AlcoholContent = dto.AlcoholContent,
                            Grape = dto.Grape,
                            Origin = dto.Origin,
                            Color = dto.Color,
                            Description = dto.Description,
                            Type = AlcoholType.Wine
                        });
                        break;
                    case "Vodka":
                        drinks.Add(new Vodka
                        {
                            Name = dto.Name,
                            AlcoholContent = dto.AlcoholContent,
                            Origin = dto.Origin,
                            Description = dto.Description,
                            Type = AlcoholType.Vodka
                        });
                        break;
                    case "Whiskey":
                        drinks.Add(new Whiskey
                        {
                            Name = dto.Name,
                            AlcoholContent = dto.AlcoholContent,
                            Origin = dto.Origin,
                            Description = dto.Description,
                            Type = AlcoholType.Whiskey
                        });
                        break;
                    default:
                        errors.Add($"Unknown drink type: {dto.Type}");
                        break;
                }
            }
            errors.ForEach(e => System.Console.WriteLine($"Mapping error: {e}"));
            return (drinks, errors);
        }

        public static List<string> ValidateDrinks(this IEnumerable<AlcoholicDrink> drinks)
        {
            var errors = new List<string>();
            foreach (var drink in drinks)
            {
                var error = drink.ValidateDrink();
                System.Console.WriteLine($"Validating drink {drink.Name}: {error ?? "valid"}");
                if (!string.IsNullOrWhiteSpace(error))
                {
                    errors.Add(string.IsNullOrWhiteSpace(drink.Name)
                        ? error
                        : $"{drink.Name}: {error}");
                }
            }
            return errors;
        }

        public static string? ValidateDrink(this AlcoholicDrink drink)
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
                    var currentYear = System.DateTime.UtcNow.Year + 1;
                    if (wine.Year < 1900 || wine.Year > currentYear)
                        return "Year must be between 1900 and next year.";
                    return null;

                case AlcoholType.Vodka:
                    if (drink is not Vodka vodka)
                        return "Type is Vodka but payload is not a Vodka.";
                    if (string.IsNullOrWhiteSpace(vodka.Distillery))
                        return "Distillery is required for vodka.";
                    return null;

                case AlcoholType.Whiskey:
                    if (drink is not Whiskey whiskey)
                        return "Type is Whiskey but payload is not a Whiskey.";
                    // Add whiskey-specific validation here if needed
                    return null;

                default:
                    return "Unsupported drink type.";
            }
        }
    }
}
