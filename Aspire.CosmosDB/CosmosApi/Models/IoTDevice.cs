namespace CosmosApi.Models;

public record IoTDevice(string Description, string id, string UserId, bool IsOnline=false)
{
    internal static string UserIdPartitionKey = "/UserId";
}
