using RateDrinksApi.Models;
using RateDrinksApi.Repositories;

namespace RateDrinksApi.Services
{
    public interface IAlcoholicDrinkService<T> where T : IAlcoholicDrink
    {
        IEnumerable<T> GetAll();
        T? GetById(int id);
        void Add(T drink);
        void Update(T drink);
        void Delete(int id);
    }
}