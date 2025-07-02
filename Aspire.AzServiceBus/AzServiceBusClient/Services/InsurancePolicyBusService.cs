using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzServiceBusClient.Services;

public class InsurancePolicyBusService : BackgroundService
{
    private readonly ServiceBusClient _serviceBusClient;
    const string queueName = "insurancepolicies";
    private readonly ILogger<InsurancePolicyBusService> _logger;
    private static readonly ActivitySource ActivitySource = new("AzServiceBusClient.InsurancePolicyBusService");


    public InsurancePolicyBusService(ServiceBusClient serviceBusClient, ILogger<InsurancePolicyBusService> logger)
    {
        _serviceBusClient = serviceBusClient;
        _logger = logger;
    }

    public async Task ReceiveMessageAsync()
    {
        var processor = _serviceBusClient.CreateProcessor(queueName, new ServiceBusProcessorOptions());

        try
        {
            processor.ProcessMessageAsync += MessageHandler;
            processor.ProcessErrorAsync += ErrorHandler;

            await processor.StartProcessingAsync();
            Console.WriteLine("Receiving messages. Press key to stop");
            Console.ReadKey();

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

        Console.WriteLine($"New Message: {body}");

        return arg.CompleteMessageAsync(arg.Message);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await ReceiveMessageAsync();
    }

    public void Dispose()
    {
        ActivitySource?.Dispose();
    }
}
