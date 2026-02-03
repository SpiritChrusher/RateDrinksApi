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


        public async Task<List<AlcoholicDrink>> GetAllAsync(AlcoholType? type = null)
        {            string sql = type.HasValue
                ? $"SELECT * FROM c WHERE c.Type = {(int)type.Value}"
                : "SELECT * FROM c";
            var query = _cosmosDb.Container.GetItemQueryIterator<AlcoholicDrink>(sql);
            var results = new List<AlcoholicDrink>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response);
            }
            return results;
        }

        public async Task<AlcoholicDrink?> GetByIdAsync(string id)
        {
            var sql = $"SELECT * FROM c WHERE c.Id = '{id}'";
            var query = _cosmosDb.Container.GetItemQueryIterator<AlcoholicDrink>(sql);
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                foreach (var drink in response)
                {
                    if (drink != null) return drink;
                }
            }
            return null;
        }

        public async Task AddAsync(AlcoholicDrink drink)
        {
            string id = drink.Type.ToString(); //$"{drink.Name}_{drink.AlcoholContent}".Replace(" ", "").Replace(".", "").ToLowerInvariant();
            drink.Id = id;
            try
            {
                //serialize to json to see the actual content being sent to Cosmos DB
                var json = System.Text.Json.JsonSerializer.Serialize(drink); 
                Console.WriteLine($"Serialized drink JSON: {json}");
                var pk = new PartitionKey(drink.Type.ToString());
                var response = await _cosmosDb.Container.UpsertItemAsync<AlcoholicDrink>(drink, pk);
                Console.WriteLine($"Item created with RU charge: {response.RequestCharge}");
            }
            catch (CosmosException cosmosEx) when (cosmosEx.StatusCode == System.Net.HttpStatusCode.Conflict)
            {
                // Item with the same Id already exists
                throw new InvalidOperationException($"A drink with Id '{id}' already exists.", cosmosEx);
            }
        }

        public async Task UpdateAsync(AlcoholicDrink drink)
        {
            await _cosmosDb.Container.UpsertItemAsync(drink, new PartitionKey(drink.Id));
        }

        public async Task DeleteAsync(string id)
        {
            await _cosmosDb.Container.DeleteItemAsync<AlcoholicDrink>(id, new PartitionKey(id));
        }
    }
}
