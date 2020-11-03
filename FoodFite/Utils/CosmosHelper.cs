namespace FoodFite.Utils
{
    using System.Threading.Tasks;
    using Microsoft.Azure.Cosmos;

    public class CosmosHelper
    {
        public async static Task<Container> GetContainerAsync(CosmosClient client, string databaseId, string containerId)
        {
            var database = client.GetDatabase(databaseId);
            Container container = await database.CreateContainerIfNotExistsAsync(new ContainerProperties(containerId, "/id"));
            return container;
        }
    }
}