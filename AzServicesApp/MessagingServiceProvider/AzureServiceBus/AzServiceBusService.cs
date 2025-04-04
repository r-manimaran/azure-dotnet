using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace MessagingServiceProvider.AzureServiceBus;

public class AzServiceBusService : IMessageService
{
    private readonly IOptions<AzureServiceBusSettings> _options;
    private readonly ILogger<AzServiceBusService> _logger;
    private readonly ServiceBusClient _client;
    private readonly ServiceBusAdministrationClient _adminClient;
    private readonly List<ServiceBusProcessor> _activeProcessors = new();
    public AzServiceBusService(IOptions<AzureServiceBusSettings> options, 
                               ILogger<AzServiceBusService> logger)
    {
        _options = options;

        _logger = logger;

        _client = new ServiceBusClient(_options.Value.ConnectionString);

        _adminClient = new ServiceBusAdministrationClient(_options.Value.ConnectionString);
    }
    
    public async Task PublishMessage<T>(string queueOrTopic, T payload, MessageType messageType)
    {
        await using var sender = _client.CreateSender(queueOrTopic);

        var message = new ServiceBusMessage(JsonSerializer.Serialize(payload))
        {
            ContentType = "application/json",
            CorrelationId = Guid.NewGuid().ToString()
        };

        _logger.LogInformation("Publishing the message with payload:{payload}", JsonSerializer.Serialize(payload));

        await sender.SendMessageAsync(message);

        _logger.LogInformation("Send Message to {Destination}:{MessageId}", queueOrTopic, message.MessageId);
    }

    //public void Subscribe(string topicOrQueue, Func<Message, Task> messageHandler)
    public async Task SubscribeAsync<T>(string subscriptionName, Func<T, ProcessMessageEventArgs, Task> handler, string filterExpression = null, CancellationToken cancellationToken = default)
    {
        var topicName = typeof(T).Name.ToLower();
        // await EnsureTopicExistsAsync(topicName, subscriptionName, filterExpression);

        var processor = _client.CreateProcessor(topicName, subscriptionName, new ServiceBusProcessorOptions
        {
            AutoCompleteMessages = false,
            MaxConcurrentCalls = 5
        });
        _activeProcessors.Add(processor);

        processor.ProcessMessageAsync += async args =>
        {
            try
            {
                var payload = JsonSerializer.Deserialize<T>(args.Message.Body.ToString());
                await handler(payload, args);
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error processing message");
                await args.AbandonMessageAsync(args.Message);
            }
        };

        processor.ProcessErrorAsync += args =>
        {
            _logger?.LogError(args.Exception, "Processor error");
            return Task.CompletedTask;
        };

        await processor.StartProcessingAsync(cancellationToken);
    }
}
