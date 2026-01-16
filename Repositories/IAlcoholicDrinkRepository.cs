using RateDrinksApi.Models;

namespace RateDrinksApi.Repositories
{
    public interface IAlcoholicDrinkRepository<T> where T : IAlcoholicDrink
    {
        IEnumerable<T> GetAll();
        T? GetById(int id);
        void Add(T drink);
        void Update(T drink);
        void Delete(int id);
    }
}