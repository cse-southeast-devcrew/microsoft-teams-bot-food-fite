namespace FoodFite.Services
{
    using System;
    using Microsoft.Azure.Cosmos;
    using FoodFite.Utils;
    using Microsoft.Extensions.Configuration;
    using System.Threading.Tasks;
    using FoodFite.Models;
    using System.Collections.Generic;
    using System.IO;
    using Newtonsoft.Json;
    using System.Linq;

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
            catch (Exception up)
            {
                throw up; // TODO: May want to do something better here...
            }

            return stateItemResponse;
        }

        public async Task<ItemResponse<T>> ReadByIdAsync(string id)
        {
            ItemResponse<T> stateItemResponse = null;

            try
            {
                Container container = await CosmosHelper.GetContainerAsync(_client, _databaseId, _containerId);
                stateItemResponse = await container.ReadItemAsync<T>(id: id, partitionKey: new PartitionKey(id));
            }
            catch
            {
                // TODO: Log this as exception or item doesn't exist
            }

            return stateItemResponse;
        }

        public async Task<List<T>> ReadAllAsync()
        {
            List<T> itemList = new List<T>();

            try
            {
                Container container = await CosmosHelper.GetContainerAsync(_client, _databaseId, _containerId);

                var query = "SELECT * FROM c";
                FeedIterator<T> resultSet = container.GetItemQueryIterator<T>(new QueryDefinition(query));
                while (resultSet.HasMoreResults)
                {
                    FeedResponse<T> response = await resultSet.ReadNextAsync();
                    itemList.AddRange(response);
                }
            }
            catch
            {
                // TODO: Log this as exception or user doesn't exist
            }

            return itemList;
        }
    }
}