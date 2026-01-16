namespace RateDrinksApi.Models
{
	public interface IAlcoholicDrink
	{
		int Id { get; set; }
		string Name { get; set; }
		double AlcoholContent { get; set; }
		string Type { get; set; }
	}
}
