using RateDrinksApi.Models;

namespace RateDrinksApi.Services;

public interface IRatingService
{
    Task AddRatingAsync(Rating rating);
    Task<IEnumerable<Rating>> GetRatingsForDrinkAsync(string drinkId);
    Task UpdateRatingAsync(Rating rating);
    Task DeleteRatingAsync(int ratingId);
}
