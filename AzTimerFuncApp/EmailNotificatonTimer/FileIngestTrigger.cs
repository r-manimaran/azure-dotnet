using System;
using System.Text.Json;
using Azure.Storage.Queues.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace EmailNotificatonTimer;

public class FileIngestTrigger
{
    private readonly ILogger<FileIngestTrigger> _logger;
    private const string QueueNameConfigKey = "QueueName";
    private const string QueueNameFromConfig = $"%{QueueNameConfigKey}%";
    public FileIngestTrigger(ILogger<FileIngestTrigger> logger)
    {
        _logger = logger;
    }

    [Function(nameof(FileIngestTrigger))]
    public void Run([QueueTrigger(QueueNameFromConfig, Connection = "QueueConnectionString")] QueueMessage message, FunctionContext executionContext)
    {
        _logger.LogInformation("C# Queue trigger function processed: {messageText}", message.MessageText);
        
    }
}