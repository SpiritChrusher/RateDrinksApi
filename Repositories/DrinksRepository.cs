using Microsoft.Azure.Cosmos;
using RateDrinksApi.Models;
using RateDrinksApi.Data;
using RateDrinksApi.Repositories;

namespace RateDrinksApi.Repositories
{
    public class DrinksRepository : IDrinksRepository
    {
        private readonly CosmosDrinksDb _cosmosDb;
        public DrinksRepository(CosmosDrinksDb cosmosDb)
        {
            _cosmosDb = cosmosDb;
        }

        public IEnumerable<AlcoholicDrink> GetAll(AlcoholType? type = null)
        {
            string sql = type.HasValue
                ? $"SELECT * FROM c WHERE c.Type = '{type.Value}'"
                : "SELECT * FROM c";
            var query = _cosmosDb.Container.GetItemQueryIterator<AlcoholicDrink>(sql);
            var results = new List<AlcoholicDrink>();
            while (query.HasMoreResults)
            {
                var response = query.ReadNextAsync().Result;
                results.AddRange(response);
            }
            return results;
        }

        public AlcoholicDrink? GetById(string id)
        {
            var sql = $"SELECT * FROM c WHERE c.Id = '{id}'";
            var query = _cosmosDb.Container.GetItemQueryIterator<AlcoholicDrink>(sql);
            while (query.HasMoreResults)
            {
                var response = query.ReadNextAsync().Result;
                foreach (var drink in response)
                {
                    if (drink != null) return drink;
                }
            }
            return null;
        }

        public void Add(AlcoholicDrink drink)
        {
            if (string.IsNullOrWhiteSpace(drink.Id))
            {
                // Simple deterministic Id: Name-Type-AlcoholContent
                drink.Id = $"{drink.Name}-{drink.Type}-{drink.AlcoholContent}".Replace(" ", "").ToLowerInvariant();
            }
            _cosmosDb.Container.CreateItemAsync(drink).Wait();
        }

        public void Update(AlcoholicDrink drink)
        {
            _cosmosDb.Container.UpsertItemAsync(drink).Wait();
        }

        public void Delete(string id)
        {
            _cosmosDb.Container.DeleteItemAsync<AlcoholicDrink>(id, new PartitionKey(id)).Wait();
        }
    }
}
