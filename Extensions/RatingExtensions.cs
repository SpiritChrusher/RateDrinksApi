namespace RateDrinksApi.Extensions;

using RateDrinksApi.Services;

public static class RatingExtensions
{
    public static async Task<double?> GetAverageRatingAsync(this IRatingService ratingService, int drinkId)
    {
        var ratings = await ratingService.GetRatingsForDrinkAsync(drinkId);
        if (ratings == null || !ratings.Any()) return null;
        return ratings.Average(r => r.Score);
    }
}
