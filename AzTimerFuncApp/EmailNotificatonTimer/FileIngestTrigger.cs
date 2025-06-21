using Azure.Storage.Blobs.Models;
using Azure.Storage.Queues.Models;
using EmailNotificatonTimer.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Text.Json;
using AzureStorageUtility;
using JsonException = Newtonsoft.Json.JsonException;
using System.Threading.Tasks;
using EmailNotificatonTimer.Services;

namespace EmailNotificatonTimer;

public class FileIngestTrigger
{
    private readonly AzureBlobStorageUtility _blobUtility;
    private readonly EmployeeService _employeeService;
    private readonly ILogger<FileIngestTrigger> _logger;
    private const string QueueNameConfigKey = "QueueName";
    private const string QueueNameFromConfig = $"%{QueueNameConfigKey}%";
    public FileIngestTrigger(
        AzureBlobStorageUtility blobUtility, 
        EmployeeService employeeService,
        ILogger<FileIngestTrigger> logger)
    {
        _blobUtility = blobUtility;
        _employeeService = employeeService;
        _logger = logger;
    }

    [Function(nameof(FileIngestTrigger))]
    public async Task Run([QueueTrigger(QueueNameFromConfig, Connection = "QueueConnectionString")] QueueMessage message, FunctionContext executionContext)
    {
        _logger.LogInformation("C# Queue trigger function processed: {messageText}", message.MessageText);
        try
        {
            // Deserialize the message content to a strongly typed object
            var eventType = new { topic = "", subject = "", eventType = "", id = "", data = new { url = "", blobType = "", eTag = "", contentLength = "" } };
            var fileIngestData = JsonConvert.DeserializeAnonymousType(message.MessageText, eventType);
            FileQueueMessage fileQueueMessage = new FileQueueMessage();
            
            if (fileIngestData?.data != null)
            {
                string blobUrl = fileIngestData.data.url;
                fileQueueMessage.BlobFileUrl = blobUrl;
                fileQueueMessage.BlobType = fileIngestData.data.blobType;
                fileQueueMessage.ContainerName = Utilities.GetContainerName(blobUrl);
                fileQueueMessage.BlobFileName = Utilities.GetBlobFileName(blobUrl);
                fileQueueMessage.BlobFileExtension = Utilities.GetBlobFileExtension(blobUrl);
                fileQueueMessage.queueMessage = message;

               
                _logger.LogInformation("File Ingest Data: {BlobFileUrl}, {BlobType}, {ContainerName}, {BlobFileName}, {BlobFileExtension}",
                    fileQueueMessage.BlobFileUrl, 
                    fileQueueMessage.BlobType, 
                    fileQueueMessage.ContainerName, 
                    fileQueueMessage.BlobFileName, 
                    fileQueueMessage.BlobFileExtension);
                // Process the file and extract the file content
                byte[] fileArray = _blobUtility.DownloadBlobAsync(fileQueueMessage.ContainerName, fileQueueMessage.BlobFileName).GetAwaiter().GetResult();

                if (fileArray != null && fileArray.Length > 0)
                {
                    _logger.LogInformation("File content downloaded successfully. Size: {size} bytes", fileArray.Length);
                    
                    // Read  the file content and convert it to Employee object
                    string fileContent = System.Text.Encoding.UTF8.GetString(fileArray);
                    _logger.LogInformation("File content: {fileContent}", fileContent);
                    if (!string.IsNullOrEmpty(fileContent))
                    {
                        _logger.LogInformation("Deserializing file content to Employee object.");
                        Employee employee = System.Text.Json.JsonSerializer.Deserialize<Employee>(fileContent);
                        if (employee != null)
                        {
                            _logger.LogInformation("Deserialized Employee: {UserName}, {Email}, {CreatedOn}", 
                                employee.UserName, 
                                employee.Email, 
                                employee.CreatedOn);

                            // insert the employee into the database
                            await _employeeService.InsertEmployeeAsync(employee);
                        }
                        else
                        {
                            _logger.LogWarning("Deserialized Employee object is null. Check the file content format.");
                        }
                    }
                    else
                    {
                        _logger.LogWarning("File content is empty or null.");
                    }                    
                }
                else
                {
                    _logger.LogWarning("Downloaded file content is empty or null.");
                }
            }
            else
            {
                _logger.LogWarning("Deserialized data is null. Check the message format.");
            }
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialize message content: {messageText}", message.MessageText);
        }

    }
}