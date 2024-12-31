using Microsoft.Azure.Cosmos;

namespace CosmosApi.Extensions;

public static class CosmosClientIoTAppExtensions
{
    public static Container GetAppDataContainer(this CosmosClient cosmosClient)
    {
        var database = cosmosClient.GetDatabase("iotdb");
        var iots = database.GetContainer("iots") ?? throw new ApplicationException("CosmosDB collection missing");
        return iots;
    }

    public static async IAsyncEnumerable<TModel> ToAsyncEnumerable<TModel>(this FeedIterator<TModel> setIterator)
    {
        while (setIterator.HasMoreResults)
        {
            foreach(var item in await setIterator.ReadNextAsync())
            {
                yield return item;
            }
        }
    }
}
