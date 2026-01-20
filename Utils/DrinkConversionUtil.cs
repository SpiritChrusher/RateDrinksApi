#if false
using RateDrinksApi.Models;
using RateDrinksApi.Data;

namespace RateDrinksApi.Utils;

public static class DrinkConversionUtil
{

    public static AlcoholicDrink? GetDrinkById(int id, DrinksDbContext db) 
    => db.Drinks.FirstOrDefault(d => d.Id == id)!.ToAlcoholicDrink();

    public static bool UpdateDrinkById(int id, AlcoholicDrink drink, DrinksDbContext db)
    {
        var existing = GetDrinkById(id, db);
        if (existing is null) return false;
        drink.Id = id;
        try
        {
            db.Drinks.Update(drink.ToDrinkRecord());
            db.SaveChanges();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static bool DeleteDrinkById(int id, DrinksDbContext db)
    {
        var drink = db.Drinks.FirstOrDefault(d => d.Id == id);
        if (drink != null) 
        {
            db.Drinks.Remove(drink); 
            db.SaveChanges();
            return true; 
        }
        
        return false;
    }
}
#endif
