using System;
using Azure.Storage.Queues.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace EmailNotificatonTimer;

public class FileIngestTrigger
{
    private readonly ILogger<FileIngestTrigger> _logger;

    public FileIngestTrigger(ILogger<FileIngestTrigger> logger)
    {
        _logger = logger;
    }

    //[Function(nameof(FileIngestTrigger))]
    public void Run([QueueTrigger("myqueue-items", Connection = "StorageConnection")] QueueMessage message)
    {
        _logger.LogInformation("C# Queue trigger function processed: {messageText}", message.MessageText);
    }
}