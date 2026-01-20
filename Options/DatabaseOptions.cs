
namespace RateDrinksApi.Options;

public class DatabaseOptions
{
    public ConnectionStringsOptions ConnectionStrings { get; set; } = new();
    public string DrinksTaleName { get; set; } = "Drinks";
    public string RatingsTableName { get; set; } = "Ratings";
}

public class ConnectionStringsOptions
{
    public string AZURE_SQL_CONNECTIONSTRING { get; set; } = string.Empty;
}
