
using AzBlobFileUploader.Models;
using AzFileProcessing.Common;
using AzFileProcessing.Common.Services;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using MessagingServiceProvider;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text;

namespace FileIngestionProcessor
{
    public class IngestionBackgroundService : BackgroundService
    {
        private readonly IOptions<AzureSetting> _azureOption;
        private readonly IServiceProvider _serviceProvider;
        private readonly QueueClient _queueClient;
        private readonly ILogger<IngestionBackgroundService> _logger;

        public IngestionBackgroundService(IOptions<AzureSetting> azureOption, 
                                          IServiceProvider serviceProvider,
                                          QueueClient queueClient, 
                                          ILogger<IngestionBackgroundService> logger)
        {
            _azureOption = azureOption;
            _serviceProvider = serviceProvider;
            _queueClient = queueClient;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Started File Ingestion Process");

            while(!stoppingToken.IsCancellationRequested)
            {
                // Get the Storage Queue Message here
                var messages = await GetStorageQueueMessages(_azureOption.Value.StorageQueueMessageCountToReceive,
                                                            _azureOption.Value.VisibilityTimeout,
                                                            stoppingToken);
                if(messages.Any())
                {
                    foreach(var message in messages)
                    {
                        var uniqueId = Guid.NewGuid().ToString();
                        var messageGuid = Guid.Parse(message.QueueMessage.MessageId);

                        var ingestionReceived = new IngestionReceived
                        {
                            Id = Guid.Parse(message.QueueMessage.MessageId),
                            ProcessReferenceId = uniqueId,
                            BlobUrl = message.BlobFileUrl,
                        };

                        using var scope = _serviceProvider.CreateScope();
                        var _eventService = scope.ServiceProvider.GetRequiredService<IEventService>();
                        var _serviceBus = scope.ServiceProvider.GetRequiredService<IMessageService>();
                        await _eventService.SaveEvent(ingestionReceived);

                        // Create the Process File Event
                        var processFileEvent = new ProcessFileEvent
                        {
                            Id = new Guid(),
                            ProcessReferenceId = uniqueId,
                            BlobUrl =message.BlobFileUrl,
                            FileName = message.BlobFileName,
                            FileExtension = message.BlobFileExtension
                        };

                        // Store the Event to Event Store
                        await _eventService.SaveEvent(processFileEvent);

                        // Publish the new Event to Azure Service Bus
                         await _serviceBus.PublishMessage(_azureOption.Value.ServiceBusTopicName, processFileEvent, MessageType.ProcessFile);

                        _logger.LogInformation("");
                    }
                }    
            }
        }

        private async Task<List<FileProcessingQueueMessage>> GetStorageQueueMessages(int storageQueueMessageCountToReceive, int visibilityTimeout, CancellationToken stoppingToken)
        {
            List<FileProcessingQueueMessage> queueMessages = new List<FileProcessingQueueMessage>();

            QueueMessage[] messages = await _queueClient.ReceiveMessagesAsync(storageQueueMessageCountToReceive,
                                                                              TimeSpan.FromSeconds(visibilityTimeout),
                                                                              stoppingToken);
            string messageText = string.Empty;
            foreach(QueueMessage message in messages)
            {
                

                FileProcessingQueueMessage queueMessage = new FileProcessingQueueMessage();
                queueMessage.QueueMessage = message;
                // Get the messageText
                messageText = Encoding.UTF8.GetString(Convert.FromBase64String(message.MessageText));

                var anonymousType = new
                {
                    topic = "",
                    subject = "",
                    eventType = "",
                    id = "",
                    eventTime = "",
                    data = new
                    {
                        url = "",
                        blobType = "",
                        contentLength = "",
                        eTag = "",
                        api = "",
                        requestId = ""
                    }
                };
                if(!string.IsNullOrEmpty(messageText))
                {
                    var eventInfo = JsonConvert.DeserializeAnonymousType(messageText, anonymousType);

                    if(eventInfo!=null)
                    {
                        string blobFileUrl = eventInfo.data.url;
                        queueMessage.BlobFileUrl = blobFileUrl;
                        queueMessage.BlobType = eventInfo.data.blobType;
                        queueMessage.ContainerName = blobFileUrl;
                        string blobFileName = blobFileUrl;
                        queueMessage.BlobFileName = blobFileName;
                        FileInfo info = new FileInfo(blobFileName);
                        queueMessage.BlobFileExtension = info.Extension;

                        queueMessages.Add(queueMessage);
                    }
                }
                
            }
            return queueMessages;
        }
    }
}
