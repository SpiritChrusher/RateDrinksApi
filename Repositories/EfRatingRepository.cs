#if false
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RateDrinksApi.Data;
using RateDrinksApi.Models;

namespace RateDrinksApi.Repositories
{
    public class EfRatingRepository : IRatingRepository
    {
        private readonly DrinksDbContext _context;

        public EfRatingRepository(DrinksDbContext context)
        {
            _context = context;
        }

        public async Task AddRatingAsync(Rating rating)
        {
            _context.Ratings.Add(rating);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Rating>> GetRatingsForDrinkAsync(int drinkId)
        {
            return await _context.Ratings
                .Where(r => r.DrinkId == drinkId)
                .ToListAsync();
        }
        public async Task UpdateRatingAsync(Rating rating)
        {
            _context.Ratings.Update(rating);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteRatingAsync(int ratingId)
        {
            var rating = await _context.Ratings.FindAsync(ratingId);
            if (rating != null)
            {
                _context.Ratings.Remove(rating);
                await _context.SaveChangesAsync();
            }
        }
    }
}
#endif
