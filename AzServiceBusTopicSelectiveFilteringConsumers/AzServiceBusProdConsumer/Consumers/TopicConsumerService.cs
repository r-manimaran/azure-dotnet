
using AzServiceBusProdConsumer.Models;
using AzServiceBusProdConsumer.Services;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace AzServiceBusProdConsumer.Consumers;

public class TopicConsumerService : BackgroundService
{
    private readonly ILogger<TopicConsumerService> _logger;
    private readonly IAzServiceBusService _azServiceBusService;
    private readonly IOptions<AzureServiceBusSettings> _option;
    private readonly IServiceProvider _serviceProvider;
    private readonly string _subscriptionName;
    public TopicConsumerService(ILogger<TopicConsumerService> logger,
                                IAzServiceBusService azServiceBusService,
                                IOptions<AzureServiceBusSettings> option,
                                IServiceProvider serviceProvider)
    {
        _logger = logger;
        _azServiceBusService = azServiceBusService;
        _option = option;
        _serviceProvider = serviceProvider;
 
        _subscriptionName = _option.Value.Subscriptions[0]!;

    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        

        await _azServiceBusService.EnsureSubscriptionExistsAsync(_subscriptionName, "messageType='inventory'");

        var processor = _azServiceBusService.CreateProcessor(_subscriptionName);

        processor.ProcessMessageAsync += Processor_ProcessMessageAsync;

        processor.ProcessErrorAsync += Processor_ProcessErrorAsync;

        await processor.StartProcessingAsync(stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }

        await processor.StartProcessingAsync();
    }

    private Task Processor_ProcessErrorAsync(Azure.Messaging.ServiceBus.ProcessErrorEventArgs arg)
    {
        _logger.LogError(arg.Exception, "Error in inventory processor");

        return Task.CompletedTask;
    }

    private async Task Processor_ProcessMessageAsync(Azure.Messaging.ServiceBus.ProcessMessageEventArgs arg)
    {
        var body = arg.Message.Body.ToString();
        _logger.LogInformation($"Inventory Service Received:{body}");

        Event newEvent = new Event
        {
            Id = new Guid(),
            UniqueId = new Guid(),
            Data = JsonSerializer.Serialize(body),
            CreatedOn = DateTime.UtcNow,
            Type = "Inventory"
        };
        using var scope = _serviceProvider.CreateScope();
        var _eventService = scope.ServiceProvider.GetRequiredService<IEventService>();

        await _eventService.CreateEvent(newEvent);

        _logger.LogInformation("Added to Event Source DB");
        // Process the inventory logic here
        await Task.Delay(500);

        await arg.CompleteMessageAsync(arg.Message);
    }
}
