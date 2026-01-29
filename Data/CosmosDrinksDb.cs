using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using RateDrinksApi.Options;

namespace RateDrinksApi.Data;

public class CosmosDrinksDb
{
    private readonly CosmosClient _client;
    private readonly Container _container;
    public CosmosDrinksDb(IOptions<CosmosDbOptions> options)
    {
        var opts = options.Value;
        _client = new CosmosClient(opts.ConnectionString);
        _container = _client.GetContainer(opts.DatabaseName, opts.DrinksContainerName);
    }

    public Container Container => _container;
    public CosmosClient Client => _client;
}
