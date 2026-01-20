using System.Collections.Generic;
using System.Threading.Tasks;
using RateDrinksApi.Models;

namespace RateDrinksApi.Repositories
{
    public interface IRatingRepository
    {
        Task AddRatingAsync(Rating rating);
        Task<IEnumerable<Rating>> GetRatingsForDrinkAsync(int drinkId);
        Task UpdateRatingAsync(Rating rating);
        Task DeleteRatingAsync(int ratingId);
    }
}
