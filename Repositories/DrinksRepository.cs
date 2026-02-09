using Microsoft.Azure.Cosmos;
using RateDrinksApi.Models;
using RateDrinksApi.Data;
using Microsoft.Extensions.Logging;

namespace RateDrinksApi.Repositories
{
    public class DrinksRepository : IDrinksRepository
    {
        private readonly CosmosDrinksDb _cosmosDb;
        private readonly ILogger<DrinksRepository> _logger;
        public DrinksRepository(CosmosDrinksDb cosmosDb, ILogger<DrinksRepository> logger)
        {
            _cosmosDb = cosmosDb;
            _logger = logger;
        }

        public async Task<List<AlcoholicDrink>> GetAllAsync(AlcoholType? type = null)
        {
            _logger.LogInformation("Querying all drinks. Type filter: {Type}", type);
            string sql = type.HasValue
                ? $"SELECT * FROM c WHERE c.Type = {(int)type.Value}"
                : "SELECT * FROM c";
            var query = _cosmosDb.Container.GetItemQueryIterator<AlcoholicDrink>(sql);
            var results = new List<AlcoholicDrink>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response);
            }
            _logger.LogInformation("Found {Count} drinks.", results.Count);
            return results;
        }

        public async Task<AlcoholicDrink?> GetByIdAsync(string id)
        {
            _logger.LogInformation("Querying drink by id: {Id}", id);
            var sql = $"SELECT * FROM c WHERE c.Id = '{id}'";
            var query = _cosmosDb.Container.GetItemQueryIterator<AlcoholicDrink>(sql);
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                foreach (var drink in response)
                {
                    if (drink != null)
                    {
                        _logger.LogInformation("Drink found: {Id}", id);
                        return drink;
                    }
                }
            }
            _logger.LogWarning("Drink not found: {Id}", id);
            return null;
        }

        public async Task AddAsync(AlcoholicDrink drink)
        {
            string id = drink.Type.ToString();
            drink.Id = id;
            try
            {
                var json = System.Text.Json.JsonSerializer.Serialize(drink);
                _logger.LogInformation("Serialized drink JSON: {Json}", json);
                var pk = new PartitionKey(drink.Type.ToString());
                var response = await _cosmosDb.Container.UpsertItemAsync<AlcoholicDrink>(drink, pk);
                _logger.LogInformation("Item created with RU charge: {RU}", response.RequestCharge);
            }
            catch (CosmosException cosmosEx) when (cosmosEx.StatusCode == System.Net.HttpStatusCode.Conflict)
            {
                _logger.LogWarning("A drink with Id '{Id}' already exists.", id);
                throw new InvalidOperationException($"A drink with Id '{id}' already exists.", cosmosEx);
            }
        }

        public async Task UpdateAsync(AlcoholicDrink drink)
        {
            _logger.LogInformation("Updating drink: {Id}", drink.Id);
            await _cosmosDb.Container.UpsertItemAsync(drink, new PartitionKey(drink.Id));
            _logger.LogInformation("Drink updated: {Id}", drink.Id);
        }

        public async Task DeleteAsync(string id)
        {
            _logger.LogInformation("Deleting drink: {Id}", id);
            await _cosmosDb.Container.DeleteItemAsync<AlcoholicDrink>(id, new PartitionKey(id));
            _logger.LogInformation("Drink deleted: {Id}", id);
        }
    }
}
