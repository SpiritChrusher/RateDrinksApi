using RateDrinksApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RateDrinksApi.Services;

public interface IRatingService
{
    Task AddRatingAsync(Rating rating);
    Task<IEnumerable<Rating>> GetRatingsForDrinkAsync(int drinkId);
    Task UpdateRatingAsync(Rating rating);
    Task DeleteRatingAsync(int ratingId);
}
