using RateDrinksApi.Models;
using RateDrinksApi.Repositories;

namespace RateDrinksApi.Services;

public class RatingService : IRatingService
{
    private readonly IRatingRepository _ratingRepository;
    private readonly ILogger<RatingService> _logger;

    public RatingService(IRatingRepository ratingRepository, ILogger<RatingService> logger)
    {
        _ratingRepository = ratingRepository;
        _logger = logger;
    }

    public async Task AddRatingAsync(Rating rating)
    {
        _logger.LogInformation("Adding rating for drink {DrinkId} by user {UserId}", rating.DrinkId, rating.UserId);
        await _ratingRepository.AddRatingAsync(rating);
        _logger.LogInformation("Rating added for drink {DrinkId}", rating.DrinkId);
    }

    public async Task<IEnumerable<Rating>> GetRatingsForDrinkAsync(string drinkId)
    {
        _logger.LogInformation("Fetching ratings for drink {DrinkId}", drinkId);
        var result = await _ratingRepository.GetRatingsForDrinkAsync(drinkId);
        _logger.LogInformation("Fetched {Count} ratings for drink {DrinkId}", result.Count(), drinkId);
        return result;
    }

    public async Task UpdateRatingAsync(Rating rating)
    {
        _logger.LogInformation("Updating rating {RatingId} for drink {DrinkId}", rating.Id, rating.DrinkId);
        await _ratingRepository.UpdateRatingAsync(rating);
        _logger.LogInformation("Rating {RatingId} updated", rating.Id);
    }

    public async Task DeleteRatingAsync(int ratingId)
    {
        _logger.LogInformation("Deleting rating {RatingId}", ratingId);
        await _ratingRepository.DeleteRatingAsync(ratingId);
        _logger.LogInformation("Rating {RatingId} deleted", ratingId);
    }
}
