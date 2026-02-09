using Microsoft.Data.SqlClient;
using RateDrinksApi.Models;
using RateDrinksApi.Options;
using Microsoft.Extensions.Options;

namespace RateDrinksApi.Repositories;

public class PgRatingRepository : IRatingRepository
{
    private readonly string _connectionString;
    private readonly string _ratingsTableName;

    private readonly ILogger<PgRatingRepository> _logger;

    public PgRatingRepository(IOptions<DatabaseOptions> dbOptions, ILogger<PgRatingRepository> logger)
    {
        _connectionString = dbOptions.Value.ConnectionString;
        _ratingsTableName = dbOptions.Value.RatingsTableName;
        _logger = logger;
    }

    public async Task AddRatingAsync(Rating rating)
    {
        _logger.LogInformation("Inserting rating for drink {DrinkId} by user {UserId}", rating.DrinkId, rating.UserId);
        using var conn = new SqlConnection(_connectionString);
        var cmd = conn.CreateCommand();
        cmd.CommandText = $@"INSERT INTO [{_ratingsTableName}] (DrinkId, UserId, Score, Comment) VALUES (@DrinkId, @UserId, @Score, @Comment)";
        cmd.Parameters.AddWithValue("@DrinkId", rating.DrinkId);
        cmd.Parameters.AddWithValue("@UserId", rating.UserId);
        cmd.Parameters.AddWithValue("@Score", rating.Score);
        cmd.Parameters.AddWithValue("@Comment", (object?)rating.Comment ?? DBNull.Value);
        await conn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
        _logger.LogInformation("Rating inserted for drink {DrinkId}", rating.DrinkId);
    }

    public async Task<IEnumerable<Rating>> GetRatingsForDrinkAsync(string drinkId)
    {
        _logger.LogInformation("Querying ratings for drink {DrinkId}", drinkId);
        var ratings = new List<Rating>();
        using var conn = new SqlConnection(_connectionString);
        var cmd = conn.CreateCommand();
        cmd.CommandText = $@"SELECT Id, DrinkId, UserId, Score, Comment FROM [{_ratingsTableName}] WHERE DrinkId = @DrinkId";
        cmd.Parameters.AddWithValue("@DrinkId", drinkId);
        await conn.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            ratings.Add(new Rating
            {
                Id = reader.GetInt32(0),
                DrinkId = reader.GetString(1),
                UserId = reader.GetString(2),
                Score = reader.GetInt32(3),
                Comment = reader.IsDBNull(4) ? string.Empty : reader.GetString(4)
            });
        }
        _logger.LogInformation("Found {Count} ratings for drink {DrinkId}", ratings.Count, drinkId);
        return ratings;
    }

    public async Task UpdateRatingAsync(Rating rating)
    {
        _logger.LogInformation("Updating rating {RatingId} for drink {DrinkId}", rating.Id, rating.DrinkId);
        using var conn = new SqlConnection(_connectionString);
        var cmd = conn.CreateCommand();
        cmd.CommandText = $@"UPDATE [{_ratingsTableName}] SET Score = @Score, Comment = @Comment WHERE Id = @Id";
        cmd.Parameters.AddWithValue("@Id", rating.Id);
        cmd.Parameters.AddWithValue("@Score", rating.Score);
        cmd.Parameters.AddWithValue("@Comment", (object?)rating.Comment ?? DBNull.Value);
        await conn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
        _logger.LogInformation("Rating {RatingId} updated", rating.Id);
    }

    public async Task DeleteRatingAsync(int ratingId)
    {
        _logger.LogInformation("Deleting rating {RatingId}", ratingId);
        using var conn = new SqlConnection(_connectionString);
        var cmd = conn.CreateCommand();
        cmd.CommandText = $@"DELETE FROM [{_ratingsTableName}] WHERE Id = @Id";
        cmd.Parameters.AddWithValue("@Id", ratingId);
        await conn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
        _logger.LogInformation("Rating {RatingId} deleted", ratingId);
    }
}
