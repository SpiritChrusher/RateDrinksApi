using RateDrinksApi.Models;

namespace RateDrinksApi.Repositories;

public interface IDrinksRepository
{
    IEnumerable<AlcoholicDrink> GetAll(AlcoholType? type = null);
    AlcoholicDrink? GetById(string id);
    void Add(AlcoholicDrink drink);
    void Update(AlcoholicDrink drink);
    void Delete(string id);
}