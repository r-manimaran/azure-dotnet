
using AzServiceBusProdConsumer.Models;
using AzServiceBusProdConsumer.Services;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace AzServiceBusProdConsumer.Consumers;

public class NotificationConsumer : IHostedService
{
    private readonly IAzServiceBusService _azServiceBusService;
    private readonly IOptions<AzureServiceBusSettings> _option;
    private readonly ILogger<NotificationConsumer> _logger;
    private readonly IServiceProvider _serviceProvider;
    private ServiceBusProcessor _processor;
    public NotificationConsumer(IAzServiceBusService azServiceBusService,
                                IOptions<AzureServiceBusSettings> option, 
                                ILogger<NotificationConsumer> logger,
                                IServiceProvider serviceProvider)
    {
        _azServiceBusService = azServiceBusService;
        _option = option;
        _logger = logger;
        _serviceProvider = serviceProvider;
    }
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _azServiceBusService.EnsureSubscriptionExistsAsync("notifications-sub", "messageType='notification'");

        _processor = _azServiceBusService.CreateProcessor("notifications-sub");

        _processor.ProcessMessageAsync += processor_ProcessMessageAsync;

        _processor.ProcessErrorAsync += processor_ProcessErrorAsync;

        await _processor.StartProcessingAsync(cancellationToken);        
    }

    private async Task processor_ProcessErrorAsync(ProcessErrorEventArgs arg)
    {
        _logger.LogError(arg.Exception, "Error in notification processor!");

        await Task.CompletedTask;
    }

    private async Task processor_ProcessMessageAsync(ProcessMessageEventArgs arg)
    {
        var body = arg.Message.Body.ToString();

        _logger.LogInformation($"Notification Service Received:{body}");

        Event newEvent = new Event
        {
            Id = new Guid(),
            UniqueId = new Guid(),
            Data = JsonSerializer.Serialize(body),
            CreatedOn = DateTime.UtcNow,
            Type = "Notification"
        };
        using var scope = _serviceProvider.CreateScope();
        var _eventService = scope.ServiceProvider.GetRequiredService<IEventService>();

        await _eventService.CreateEvent(newEvent);

        await Task.Delay(300);

        await arg.CompleteMessageAsync(arg.Message);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _processor.StopProcessingAsync();

        await _processor.DisposeAsync();
    }
}
