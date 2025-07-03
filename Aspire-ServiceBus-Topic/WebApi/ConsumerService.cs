
using Azure.Messaging.ServiceBus;
using System.Diagnostics;

namespace WebApi;

public class ConsumerService : BackgroundService
{
    private readonly ServiceBusClient _serviceBusClient;
    private readonly ILogger<ConsumerService> _logger;
    private static readonly ActivitySource ActivitySource = new("AzServiceBusClient.Topic");

    public ConsumerService(ServiceBusClient serviceBusClient, ILogger<ConsumerService> logger)
    {
        _serviceBusClient = serviceBusClient;
        _logger = logger;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await ReceiveMessageAsync(stoppingToken);
    }

    public async Task ReceiveMessageAsync(CancellationToken stoppingToken)
    {
        var processor = _serviceBusClient.CreateProcessor("insurance","sub1", new ServiceBusProcessorOptions());

        try
        {
            processor.ProcessMessageAsync += MessageHandler;
            processor.ProcessErrorAsync += ErrorHandler;

            await processor.StartProcessingAsync();
            Console.WriteLine("Receiving messages. Press key to stop");
           
            // Wait until the service is stopped
           await Task.Delay(Timeout.Infinite, stoppingToken);

            await processor.StopProcessingAsync();
        }
        catch (Exception ex)
        {
            throw;
        }
        finally
        {
            await processor.DisposeAsync();
        }
    }

    private Task ErrorHandler(ProcessErrorEventArgs arg)
    {
        _logger.LogInformation($"Error:{arg.Exception}");

        return Task.CompletedTask;
    }

    private Task MessageHandler(ProcessMessageEventArgs arg)
    {
        using var activity = ActivitySource.StartActivity("ProcessMessage");

        string body = arg.Message.Body.ToString();

        // Add message details to the activity
        activity?.SetTag("message.body", body);
        activity?.SetTag("message.id", arg.Message.MessageId);

        Console.WriteLine($"New Message Consumed: {body}");

        return arg.CompleteMessageAsync(arg.Message);
    }
}
