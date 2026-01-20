using RateDrinksApi.Models;
using RateDrinksApi.Repositories;

namespace RateDrinksApi.Services;

public class RatingService : IRatingService
{
    private readonly IRatingRepository _ratingRepository;

    public RatingService(IRatingRepository ratingRepository)
    {
        _ratingRepository = ratingRepository;
    }

    public async Task AddRatingAsync(Rating rating)
    {
        await _ratingRepository.AddRatingAsync(rating);
    }

    public async Task<IEnumerable<Rating>> GetRatingsForDrinkAsync(int drinkId)
    {
        return await _ratingRepository.GetRatingsForDrinkAsync(drinkId);
    }

    public async Task UpdateRatingAsync(Rating rating)
    {
        await _ratingRepository.UpdateRatingAsync(rating);
    }

    public async Task DeleteRatingAsync(int ratingId)
    {
        await _ratingRepository.DeleteRatingAsync(ratingId);
    }
}
