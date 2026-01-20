using RateDrinksApi.Models;

namespace RateDrinksApi.Utils;

public static class DrinkConversion
{
    public static AlcoholicDrink ToAlcoholicDrink(this DrinkRecord record)
    {
        return record.Type switch
        {
            AlcoholType.Beer => new Beer
            {
                Id = record.Id,
                Name = record.Name,
                AlcoholContent = record.AlcoholContent,
                Type = record.Type,
                Brewery = record.Brewery ?? string.Empty,
                BeerType = record.BeerType ?? BeerType.Other
            },
            AlcoholType.Wine => new Wine
            {
                Id = record.Id,
                Name = record.Name,
                AlcoholContent = record.AlcoholContent,
                Type = record.Type,
                WineType = record.WineType ?? WineType.Other,
                Vineyard = record.Vineyard ?? string.Empty,
                Year = record.Year ?? -1
            },
            AlcoholType.Vodka => new Vodka
            {
                Id = record.Id,
                Name = record.Name,
                AlcoholContent = record.AlcoholContent,
                Type = record.Type,
                VodkaType = record.VodkaType ?? VodkaType.Other,
            },
            _ => throw new NotSupportedException($"Alcohol type {record.Type} is not supported.")
        };
    }

    public static DrinkRecord ToDrinkRecord(this AlcoholicDrink drink)
    {
        var record = new DrinkRecord
        {
            Id = drink.Id,
            Name = drink.Name,
            AlcoholContent = drink.AlcoholContent,
            Type = drink.Type
        };

        switch (drink)
        {
            case Beer beer:
                record.BeerType = beer.BeerType;
                record.Brewery = beer.Brewery;
                break;
            case Wine wine:
                record.WineType = wine.WineType;
                record.Vineyard = wine.Vineyard;
                record.Year = wine.Year;
                break;
            case Vodka vodka:
                record.VodkaType = vodka.VodkaType;
                record.Distillery = vodka.Distillery;
                break;
            default:
                throw new NotSupportedException($"Alcohol type {drink.Type} is not supported.");
        }

        return record;
    }
}