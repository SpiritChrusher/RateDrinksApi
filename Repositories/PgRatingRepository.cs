using Microsoft.EntityFrameworkCore;
using RateDrinksApi.Models;
using RateDrinksApi.Data;

namespace RateDrinksApi.Repositories;

public class PgRatingRepository : IRatingRepository
{
    private readonly DrinksDbContext _db;

    public PgRatingRepository(DrinksDbContext db)
    {
        _db = db;
    }

    public async Task AddRatingAsync(Rating rating)
    {
        _db.Ratings.Add(rating);
        await _db.SaveChangesAsync();
    }

    public async Task<IEnumerable<Rating>> GetRatingsForDrinkAsync(int drinkId)
    {
        return await _db.Ratings.Where(r => r.DrinkId == drinkId).ToListAsync();
    }

    public async Task UpdateRatingAsync(Rating rating)
    {
        _db.Ratings.Update(rating);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteRatingAsync(int ratingId)
    {
        var rating = await _db.Ratings.FindAsync(ratingId);
        if (rating != null)
        {
            _db.Ratings.Remove(rating);
            await _db.SaveChangesAsync();
        }
    }
}
