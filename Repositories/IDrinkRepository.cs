using RateDrinksApi.Models;
using System.Collections.Generic;

namespace RateDrinksApi.Repositories
{
    public interface IDrinkRepository
    {
        IEnumerable<DrinkRecord> GetAll();
        DrinkRecord? GetById(int id);
        void Add(DrinkRecord drink);
        void Update(DrinkRecord drink);
        void Delete(int id);
    }
}
