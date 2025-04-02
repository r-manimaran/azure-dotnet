using AzServiceBusProdConsumer.Services;
using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AzServiceBusProdConsumer.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TriggerMessageController : ControllerBase
{
    private readonly IAzServiceBusService _azServiceBusService;
    private readonly ILogger<TriggerMessageController> _logger;

    public TriggerMessageController(IAzServiceBusService azServiceBusService, ILogger<TriggerMessageController> logger)
    {
        _azServiceBusService = azServiceBusService;
        _logger = logger;
    }

    [HttpPost("send-messages")]
    public async Task<IActionResult> SendMessageToAzTopic()
    {
        await _azServiceBusService.EnsureTopicExistsAsync();

        var sender = _azServiceBusService.CreateSender();

        // Send 10 message with different types
        var tasks = new List<Task>();
        for(int i = 1; i <= 10; i++)
        {
            var messageType = i % 2 == 0 ? "inventory" : "notification";
            var message = new ServiceBusMessage($"Message {i} - {DateTime.UtcNow}")
            {
                ApplicationProperties = { ["messageType"] = messageType }
            };

            tasks.Add(sender.SendMessageAsync(message));
            _logger.LogInformation($"Sent message {i} of type {messageType}");

        }

        await Task.WhenAll(tasks);
        return Ok("10 test message sent to topic");

    }
}
