using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace AzServiceBusProdConsumer.Services;

public class AzServiceBusService : IAzServiceBusService
{
    private readonly IOptions<AzureServiceBusSettings> _options;
    private readonly ILogger<AzServiceBusService> _logger;
    private readonly ServiceBusClient _client;
    private readonly ServiceBusAdministrationClient _adminClient;
    private readonly List<ServiceBusProcessor> _activeProcessors = new();
    public AzServiceBusService(IOptions<AzureServiceBusSettings> options, ILogger<AzServiceBusService> logger)
    {
        _options = options;
        _logger = logger;
        _client = new ServiceBusClient(_options.Value.ConnectionString);
        _adminClient = new ServiceBusAdministrationClient(_options.Value.ConnectionString);
    }

    public async Task EnsureTopicExistsAsync()
    {
        var adminClient = new ServiceBusAdministrationClient(_options.Value.ConnectionString);
        if (!await adminClient.TopicExistsAsync(_options.Value.TopicName))
        {
            await adminClient.CreateTopicAsync(_options.Value.TopicName);
        }
    }

    public async Task EnsureSubscriptionExistsAsync(string subscriptionName, string filter = null)
    {
        var adminClient = new ServiceBusAdministrationClient(_options.Value.ConnectionString);

        // Check if the topic exists
        //await EnsureTopicExistsAsync();

        if (!await adminClient.SubscriptionExistsAsync(_options.Value.TopicName, subscriptionName))
        {
            var options = new CreateSubscriptionOptions(_options.Value.TopicName, subscriptionName);
            if (!string.IsNullOrEmpty(filter))
            {
                var ruleOptions = new CreateRuleOptions("default", new SqlRuleFilter(filter));

                await adminClient.CreateSubscriptionAsync(options, ruleOptions);
            }
            else
            {
                await adminClient.CreateSubscriptionAsync(options);
            }
        }

    }

    public ServiceBusSender CreateSender() =>
        new ServiceBusClient(_options.Value.ConnectionString).CreateSender(_options.Value.TopicName);

    public ServiceBusProcessor CreateProcessor(string subscriptionName) =>
        new ServiceBusClient(_options.Value.ConnectionString).CreateProcessor(_options.Value.TopicName, subscriptionName);

    public async Task SendMessageAsync<T>(T payload, string queueOrTopicName, IDictionary<string, object> properties = null)
    {
        await using var sender = _client.CreateSender(queueOrTopicName);

        var message = new ServiceBusMessage(JsonSerializer.Serialize(payload))
        {
            ContentType = "application/json",
            CorrelationId = Guid.NewGuid().ToString()
        };

        // set the properties if exists
        if (properties != null)
        {
            foreach (var prop in properties)
            {
                message.ApplicationProperties.Add(prop);
            }
        }

        await sender.SendMessageAsync(message);
        _logger.LogDebug("Send Message to  {Destination}:{MessageId}", queueOrTopicName, message.MessageId);
    }

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

    public Task UnsubscribeAsync(string subscriptionName, string topicName = null)
    {
        throw new NotImplementedException();
    }

    public Task StopProcessingAsync(string subscriptionName, string topicName = null)
    {
        throw new NotImplementedException();
    }


}
