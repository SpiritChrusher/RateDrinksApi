namespace RateDrinksApi.Options;

public class CosmosDbOptions
{
    public string ConnectionString { get; set; } = string.Empty;
    public string Uri { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;
    public string DrinksContainerName { get; set; } = string.Empty;
}
