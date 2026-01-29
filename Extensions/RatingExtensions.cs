namespace RateDrinksApi.Extensions;

using RateDrinksApi.Services;

public static class RatingExtensions
{
    public static async Task<double?> GetAverageRatingAsync(this IRatingService ratingService, string drinkId)
    {
        var ratings = await ratingService.GetRatingsForDrinkAsync(drinkId);
        if (ratings is null || !ratings.Any()) 
            return null;
            
        return ratings.Average(r => r.Score);
    }
}
