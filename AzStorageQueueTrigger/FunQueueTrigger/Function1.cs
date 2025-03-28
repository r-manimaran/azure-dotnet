using System;
using Azure.Storage.Queues.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Azure.Data.Tables;
using System.Threading.Tasks;
using System.Text.Json;

namespace FunQueueTrigger
{
    public class Function1
    {
        private readonly ILogger<Function1> _logger;
        private readonly TableServiceClient _tableServiceClient;

        public Function1(ILogger<Function1> logger)
        {
            _logger = logger;
            string storageTableConnectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage")!;
           _tableServiceClient = new TableServiceClient(storageTableConnectionString);
        }

        [Function(nameof(Function1))]
        public async Task Run([QueueTrigger("myqueue-items", Connection = "AzureWebJobsStorage")] QueueMessage message)
        {
            try
            {
                _logger.LogInformation($"C# Queue trigger function processed: {message.MessageText}");

                var tableClient = _tableServiceClient.GetTableClient("MyTable");
                await tableClient.CreateIfNotExistsAsync();

                //Parse the message
                var messageData = JsonSerializer.Deserialize<MessageData>(message.MessageText);
                if (messageData == null)
                    throw new Exception("Not Valid");
                //Create table Entity
                var entity = new TableEntity(messageData.PartitionKey, messageData.RowKey)
                {
                    {"Name", messageData.Name},
                    {"Description",messageData.Description},
                    {"Timestamp",DateTimeOffset.UtcNow}
                };


                //Add Entity to tabl
                await tableClient.AddEntityAsync(entity);
                _logger.LogInformation($"Entity added to table: {messageData.Name}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error processing message: {ex.Message}");
            }
        }
    }

    public class MessageData
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
