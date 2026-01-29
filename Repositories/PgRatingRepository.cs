using Microsoft.Data.SqlClient;
using RateDrinksApi.Models;
using RateDrinksApi.Options;
using Microsoft.Extensions.Options;

namespace RateDrinksApi.Repositories;

public class PgRatingRepository : IRatingRepository
{
    private readonly string _connectionString;
    private readonly string _ratingsTableName;

    public PgRatingRepository(IOptions<DatabaseOptions> dbOptions)
    {
        _connectionString = dbOptions.Value.ConnectionString;
        _ratingsTableName = dbOptions.Value.RatingsTableName;
    }

    public async Task AddRatingAsync(Rating rating)
    {
        using var conn = new SqlConnection(_connectionString);
        var cmd = conn.CreateCommand();
        cmd.CommandText = $@"INSERT INTO [{_ratingsTableName}] (DrinkId, UserId, Score, Comment) VALUES (@DrinkId, @UserId, @Score, @Comment)";
        cmd.Parameters.AddWithValue("@DrinkId", rating.DrinkId);
        cmd.Parameters.AddWithValue("@UserId", rating.UserId);
        cmd.Parameters.AddWithValue("@Score", rating.Score);
        cmd.Parameters.AddWithValue("@Comment", (object?)rating.Comment ?? DBNull.Value);
        await conn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task<IEnumerable<Rating>> GetRatingsForDrinkAsync(string drinkId)
    {
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
        return ratings;
    }

    public async Task UpdateRatingAsync(Rating rating)
    {
        using var conn = new SqlConnection(_connectionString);
        var cmd = conn.CreateCommand();
        cmd.CommandText = $@"UPDATE [{_ratingsTableName}] SET Score = @Score, Comment = @Comment WHERE Id = @Id";
        cmd.Parameters.AddWithValue("@Id", rating.Id);
        cmd.Parameters.AddWithValue("@Score", rating.Score);
        cmd.Parameters.AddWithValue("@Comment", (object?)rating.Comment ?? DBNull.Value);
        await conn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task DeleteRatingAsync(int ratingId)
    {
        using var conn = new SqlConnection(_connectionString);
        var cmd = conn.CreateCommand();
        cmd.CommandText = $@"DELETE FROM [{_ratingsTableName}] WHERE Id = @Id";
        cmd.Parameters.AddWithValue("@Id", ratingId);
        await conn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
    }
}
