namespace RateDrinksApi.Models.Dto;

public class AlcoholicDrinkCosmosDto
{
    public string Id { get; set; } = string.Empty; // Calculated: Name + AlcoholContent
    public string Name { get; set; } = string.Empty;
    public double AlcoholContent { get; set; }
    public string Type { get; set; } = string.Empty;
}

public class BeerCosmosDto : AlcoholicDrinkCosmosDto
{
    public string? BeerType { get; set; }
    public string? Brewery { get; set; }
}

public class WineCosmosDto : AlcoholicDrinkCosmosDto
{
    public string? WineType { get; set; }
    public string? Vineyard { get; set; }
    public int? Year { get; set; }
}

public class VodkaCosmosDto : AlcoholicDrinkCosmosDto
{
    public string? VodkaType { get; set; }
    public string? Distillery { get; set; }
}
