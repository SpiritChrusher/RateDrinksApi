namespace RateDrinksApi.Models
{

    public enum BeerType
    {
        Lager,
        Ale,
        Stout,
        ImperialStout,
        IPA,
        ImperialIPA,
        PaleAle,
        Pilsner,
        Porter,
        Wheat,
        Other
    }

    public class Beer : AlcoholicDrink
    {
        public BeerType BeerType { get; set; }
        public string Brewery { get; set; } = string.Empty;
    }


    public enum WineType
    {
        Red,
        White,
        Rose,
        Sparkling,
        Dessert,
        Fortified,
        Other
    }

    public class Wine : AlcoholicDrink
    {
        public WineType WineType { get; set; }
        public string Vineyard { get; set; } = string.Empty;
        public int Year { get; set; }
    }


    public enum VodkaType
    {
        Plain,
        Flavored,
        Premium,
        Other
    }

    public class Vodka : AlcoholicDrink
    {
        public VodkaType VodkaType { get; set; }
        public string Distillery { get; set; } = string.Empty;
    }
}