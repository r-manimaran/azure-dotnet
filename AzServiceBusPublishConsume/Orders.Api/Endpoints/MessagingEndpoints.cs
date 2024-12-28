using Azure.Messaging.ServiceBus;

namespace Orders.Api.Endpoints;

internal static class MessagingEndpoints
{
    internal static WebApplication MapMessagingEndpoints(this WebApplication app) 
    {
        var endpoint = app.MapGroup("api/Messages");
        endpoint.MapPost("/SendMessage/{count}", PublishMessage);

        return app;
    }

    private static async Task<bool> PublishMessage(int count)
    {
        ServiceBusClient client = new("Endpoint=sb://127.0.0.1;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=SAS_KEY_VALUE;UseDevelopmentEmulator=true;");
        var sender = client.CreateSender("queue.1");
        using var messageBatch = await sender.CreateMessageBatchAsync();
        for(int i = 0; i <= count; i++)
        {
            if(!messageBatch.TryAddMessage(new ServiceBusMessage($"Message {i}")))
            {
                throw new Exception($"The message {i} is too large to fit in the batch");
            }
        }

        try
        {
            await sender.SendMessagesAsync(messageBatch);
            Console.WriteLine($" A batch of {count} messages has been published to ServiceBus");
        }
        finally
        {
            await sender.DisposeAsync();
            await client.DisposeAsync();
        }
        return true;
    }
}
