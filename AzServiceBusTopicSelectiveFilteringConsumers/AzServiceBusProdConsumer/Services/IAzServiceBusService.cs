using Azure.Messaging.ServiceBus;

namespace AzServiceBusProdConsumer.Services
{
    public interface IAzServiceBusService
    {
        ServiceBusProcessor CreateProcessor(string subscriptionName);
        ServiceBusSender CreateSender();
        Task EnsureSubscriptionExistsAsync(string subscriptionName, string filter = null);
        Task EnsureTopicExistsAsync();

        Task SendMessageAsync<T>(T payload, string queueOrTopicName, IDictionary<string, object> properties = null);

        //Task SubscribeAsync<T>(string subscriptionName, Func<ServiceBusReceivedMessage, Task> handler, string filterExpression = null,
        //                       CancellationToken cancellationToken = default);

        Task SubscribeAsync<T>(string subscriptionName, Func<T, ProcessMessageEventArgs, Task> handler, string filterExpression = null, CancellationToken cancellationToken = default);
        Task UnsubscribeAsync(string subscriptionName, string topicName = null);
        Task StopProcessingAsync(string subscriptionName, string topicName = null);
    }
}