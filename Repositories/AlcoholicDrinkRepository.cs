using RateDrinksApi.Models;

namespace RateDrinksApi.Repositories
{
    // The IAlcoholicDrinkRepository interface has been moved to its own file.
    // This class implements the interface for in-memory storage.
    public class InMemoryAlcoholicDrinkRepository<T> : IAlcoholicDrinkRepository<T> where T : IAlcoholicDrink
    {
        private readonly List<T> _drinks = [];
        private int _nextId = 1;

        public IEnumerable<T> GetAll()
        {
            Console.WriteLine($"[DEBUG] GetAll called. Drinks count: {_drinks.Count}");
            foreach (var d in _drinks)
                Console.WriteLine($"[DEBUG] Drink: Id={d.Id}, Name={d.Name}");
            return _drinks;
        }

        public T? GetById(int id) => _drinks.FirstOrDefault(d => d.Id == id);

        public void Add(T drink)
        {
            drink.Id = _nextId++;
            _drinks.Add(drink);
            Console.WriteLine($"[DEBUG] Add called. Added: Id={drink.Id}, Name={drink.Name}. Total now: {_drinks.Count}");
        }

        public void Update(T drink)
        {
            var index = _drinks.FindIndex(d => d.Id == drink.Id);
            if (index != -1)
                _drinks[index] = drink;
        }

        public void Delete(int id)
        {
            var drink = GetById(id);
            if (drink != null)
                _drinks.Remove(drink);
        }
    }
}