using RateDrinksApi.Models;

namespace RateDrinksApi.Repositories;

using System.Threading.Tasks;
using System.Collections.Generic;

public interface IDrinksRepository
{
    Task<List<AlcoholicDrink>> GetAllAsync(AlcoholType? type = null);
    Task<AlcoholicDrink?> GetByIdAsync(string id);
    Task AddAsync(AlcoholicDrink drink);
    Task UpdateAsync(AlcoholicDrink drink);
    Task DeleteAsync(string id);
}