using RateDrinksApi.Models;
using RateDrinksApi.Repositories;

namespace RateDrinksApi.Services
{
    public class AlcoholicDrinkService<T> where T : IAlcoholicDrink
    {
        private readonly IAlcoholicDrinkRepository<T> _repository;

        public AlcoholicDrinkService(IAlcoholicDrinkRepository<T> repository)
        {
            _repository = repository;
        }

        public IEnumerable<T> GetAll() => _repository.GetAll();
        public T? GetById(int id) => _repository.GetById(id);
        public void Add(T drink) => _repository.Add(drink);
        public void Update(T drink) => _repository.Update(drink);
        public void Delete(int id) => _repository.Delete(id);
    }
}