using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureStorageUtility;

public class AzureQueueUtility
{
    private readonly ILogger<AzureQueueUtility> _logger;

    public AzureQueueUtility(ILogger<AzureQueueUtility> logger)
    {
        _logger = logger;
    }

    public async Task EnsureQueueAsync(string queueName)
    {
        // Logic to ensure the queue exists
        _logger.LogInformation($"Ensuring queue '{queueName}' exists.");
        // Simulate async operation
        await Task.Delay(100);
        _logger.LogInformation($"Queue '{queueName}' is ready for use.");
    }

    public async Task<bool> IsQueueExists(string queueName)
    {
        // Logic to check if the queue exists
        _logger.LogInformation($"Checking if queue '{queueName}' exists.");
        // Simulate async operation
        await Task.Delay(100);
        bool exists = true; // Simulated existence check
        _logger.LogInformation($"Queue '{queueName}' exists: {exists}");
        return exists;
    }

}
