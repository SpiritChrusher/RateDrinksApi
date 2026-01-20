using Microsoft.EntityFrameworkCore;
using RateDrinksApi.Models;

namespace RateDrinksApi.Data
{

public class DrinksDbContext : DbContext
{
    private readonly string _drinksTableName;
    private readonly string _ratingsTableName;

    public DrinksDbContext(DbContextOptions<DrinksDbContext> options, Microsoft.Extensions.Options.IOptions<RateDrinksApi.Options.DatabaseOptions> dbOptions)
        : base(options)
    {
        _drinksTableName = dbOptions.Value.DrinksTaleName;
        _ratingsTableName = dbOptions.Value.RatingsTableName;
    }

    public DbSet<DrinkRecord> Drinks { get; set; }
    public DbSet<Rating> Ratings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<DrinkRecord>().ToTable(_drinksTableName);
        modelBuilder.Entity<Rating>().ToTable(_ratingsTableName);
    }
}
}
