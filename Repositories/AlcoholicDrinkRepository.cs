#if false
using RateDrinksApi.Models;
using RateDrinksApi.Data;
using Microsoft.EntityFrameworkCore;

namespace RateDrinksApi.Repositories
{
    public class EfDrinkRepository : IDrinkRepository
    {
        private readonly DrinksDbContext _db;

        public EfDrinkRepository(DrinksDbContext db)
        {
            _db = db;
        }

        public IEnumerable<DrinkRecord> GetAll()
        {
            return _db.Drinks.AsNoTracking().ToList();
        }

        public DrinkRecord? GetById(int id)
        {
            return _db.Drinks.AsNoTracking().FirstOrDefault(d => d.Id == id);
        }

        public void Add(DrinkRecord drink)
        {
            _db.Drinks.Add(drink);
            _db.SaveChanges();
        }

        public void Update(DrinkRecord drink)
        {
            _db.Drinks.Update(drink);
            _db.SaveChanges();
        }

        public void Delete(int id)
        {
            var drink = _db.Drinks.FirstOrDefault(d => d.Id == id);
            if (drink != null)
            {
                _db.Drinks.Remove(drink);
                _db.SaveChanges();
            }
        }
    }
}
#endif