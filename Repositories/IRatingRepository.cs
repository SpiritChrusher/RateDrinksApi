using RateDrinksApi.Models;

namespace RateDrinksApi.Repositories;

public interface IRatingRepository
{
    Task AddRatingAsync(Rating rating);
    Task<IEnumerable<Rating>> GetRatingsForDrinkAsync(string drinkId);
    Task UpdateRatingAsync(Rating rating);
    Task DeleteRatingAsync(int ratingId);
}
