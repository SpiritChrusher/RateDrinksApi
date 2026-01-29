namespace RateDrinksApi.Options;

public class DatabaseOptions
{
    public string ConnectionString { get; set; } = string.Empty;
    public string DrinksTaleName { get; set; } = string.Empty;
    public string RatingsTableName { get; set; } = string.Empty;
}
