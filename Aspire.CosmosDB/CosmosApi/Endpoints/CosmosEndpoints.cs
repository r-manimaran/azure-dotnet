using CosmosApi.Extensions;
using CosmosApi.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;

namespace CosmosApi.Endpoints
{
    public static class CosmosEndpoints
    {
        public static void MapCosmosEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("").WithTags("Cosmos");

            group.MapPost("/iots", async (IoTDevice iot, CosmosClient cosmosClient) =>
                (await cosmosClient.GetAppDataContainer().CreateItemAsync(iot)).Resource
             );

            group.MapGet("/iots", (CosmosClient cosmosClient) =>
                cosmosClient.GetAppDataContainer()
                            .GetItemLinqQueryable<IoTDevice>(
                                requestOptions: new()
                                {
                                    MaxItemCount = 10,
                                })
                            .ToFeedIterator()
                            .ToAsyncEnumerable()

            );

            group.MapPut("/iots", async (string id, IoTDevice iot, CosmosClient cosmosClient) =>
                (await cosmosClient.GetAppDataContainer().ReplaceItemAsync(iot,id)).Resource
            );

            group.MapDelete("/iots/{userId}/{id}", async (string userId, string id, CosmosClient cosmosClient) =>
            {
                await cosmosClient.GetAppDataContainer().DeleteItemAsync<IoTDevice>(id, new PartitionKey(userId));
                return Results.Ok();
            });
        }
    }
}
