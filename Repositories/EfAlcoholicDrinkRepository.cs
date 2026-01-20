#if false
using RateDrinksApi.Models;
using RateDrinksApi.Data;
using Microsoft.EntityFrameworkCore;

namespace RateDrinksApi.Repositories;

public class EfAlcoholicDrinkRepository<T> : IAlcoholicDrinkRepository<T> where T : AlcoholicDrink
{
    private readonly DrinksDbContext _db;
    public EfAlcoholicDrinkRepository(DrinksDbContext db)
    {
        _db = db;
    }

    public IEnumerable<T> GetAll() => _db.Set<T>().ToList();
    public T? GetById(int id) => _db.Set<T>().FirstOrDefault(d => d.Id == id);
    public void Add(T drink)
    {
        _db.Set<T>().Add(drink);
        _db.SaveChanges();
    }
    public void Update(T drink)
    {
        _db.Set<T>().Update(drink);
        _db.SaveChanges();
    }
    public void Delete(int id)
    {
        var drink = GetById(id);
        if (drink != null)
        {
            _db.Set<T>().Remove(drink);
            _db.SaveChanges();
        }
    }
}
#endif
