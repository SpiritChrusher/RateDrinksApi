namespace RateDrinksApi.Models
{
    public interface IBeer : IAlcoholicDrink
    {
        string Brewery { get; set; }
    }

    public interface IWine : IAlcoholicDrink
    {
        string Vineyard { get; set; }
        int Year { get; set; }
    }

    public interface IVodka : IAlcoholicDrink
    {
        string Distillery { get; set; }
    }
}