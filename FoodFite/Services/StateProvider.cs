namespace FoodFite.Services
{
    using System;
    using Microsoft.Azure.Cosmos;
    using FoodFite.Utils;
    using Microsoft.Extensions.Configuration;
    using System.Threading.Tasks;
    using FoodFite.Models;

    public class StateProvider<T> where T: IStateModel
    {
        protected readonly CosmosClient _client;
        protected readonly string _cosmosEndpointUri;
        protected readonly string _cosmosPrimaryKey;
        protected readonly IConfiguration _configuration;
        protected readonly string _databaseId;
        protected readonly string _containerId;

        public StateProvider(IConfiguration configuration)
        {
            _configuration = configuration;
            _cosmosEndpointUri = _configuration.GetValue<string>(EnvironmentConstants.CosmosEndpoint);
            _cosmosPrimaryKey = _configuration.GetValue<string>(EnvironmentConstants.CosmosPrimaryKey);
            _client = new CosmosClient(_cosmosEndpointUri, _cosmosPrimaryKey);
            _databaseId = _configuration.GetValue<string>(EnvironmentConstants.CosmosDatabaseId);
            _containerId = typeof(T).Name;
        }

        public async Task<ItemResponse<T>> UpsertAsync(T stateItem)
        {
            ItemResponse<T> stateItemResponse = null;

            try
            {
                Container container = await CosmosHelper.GetContainerAsync(_client, _databaseId, _containerId);
                stateItemResponse = await container.UpsertItemAsync<T>(item: stateItem, partitionKey: new PartitionKey(stateItem.Id));
            }
            catch (System.Exception up)
            {
                throw up;
            }

            return stateItemResponse;
        }

        public async Task<ItemResponse<T>> ReadByIdAsync(T stateItem)
        {
            ItemResponse<T> stateItemResponse = null;

            try
            {
                Container container = await CosmosHelper.GetContainerAsync(_client, _databaseId, _containerId);
                stateItemResponse = await container.ReadItemAsync<T>(id: stateItem.Id, partitionKey: new PartitionKey(stateItem.Id));
            }
            catch (System.Exception up)
            {
                // TODO: Log this as exception or user doesn't exist
            }

            return stateItemResponse;
        }
    }
}