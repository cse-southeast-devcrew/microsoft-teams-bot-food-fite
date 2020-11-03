namespace FoodFite.Services
{
    using System;
    using Microsoft.Azure.Cosmos;
    using FoodFite.Utils;
    using Microsoft.Extensions.Configuration;
    using System.Threading.Tasks;
    using FoodFite.Models;

    public class GameStateProvider
    {
        private readonly CosmosClient _client;
        private readonly string _cosmosEndpointUri;
        private readonly string _cosmosPrimaryKey;
        private readonly IConfiguration _configuration;

        public GameStateProvider(IConfiguration configuration)
        {
            _configuration = configuration;
            _cosmosEndpointUri = _configuration.GetValue<string>(EnvironmentConstants.CosmosEndpoint);
            _cosmosPrimaryKey = _configuration.GetValue<string>(EnvironmentConstants.CosmosPrimaryKey);
            _client = new CosmosClient(_cosmosEndpointUri, _cosmosPrimaryKey);
        }

        public async Task<ItemResponse<Cafeteria>> SaveCafeteriaAsync(Cafeteria cafeteria)
        {
            ItemResponse<Cafeteria> cafeteriaResponse = null;

            try
            {
                var container = _client.GetContainer(
                    _configuration.GetValue<string>(EnvironmentConstants.CosmosDatabaseId),
                    _configuration.GetValue<string>(EnvironmentConstants.CosmosContainerId));
                cafeteriaResponse = await container.UpsertItemAsync<Cafeteria>(item: cafeteria, partitionKey: new PartitionKey(cafeteria.Id));
            }
            catch (System.Exception up)
            {
                throw up;
            }

            return cafeteriaResponse;
        }
    }
}