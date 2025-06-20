using System;
using System.Diagnostics;
using System.Text.Json;
using System.Threading.Tasks;
using AzureStorageUtility;
using Bogus;
using EmailNotificatonTimer.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace EmailNotificatonTimer;

public class EmailNotificationTimer
{
    private readonly AzureBlobStorageUtility _blobUtility;
    private readonly ILogger<EmailNotificationTimer> _logger;
    private Faker _faker = new Faker();

    public EmailNotificationTimer(AzureBlobStorageUtility blobUtility, ILogger<EmailNotificationTimer> logger)
    {
        _blobUtility = blobUtility;
        _logger = logger;
    }

    [Function("EmailNotificationTimerFunction")]
    public async Task Run([TimerTrigger("%ExecutionScheduleTime%", RunOnStartup = true)] TimerInfo myTimer, FunctionContext executionContext)
    {
        try
        {
            DateTime dateTimeNow = DateTime.Now;
            LogAboutFunction(_logger);
            var interval = Environment.GetEnvironmentVariable("ExecutionScheduleTime") ?? "5 minutes";
            var lastUpdatedTimerTrigger = myTimer.ScheduleStatus?.LastUpdated.ToString();

            _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            if (myTimer.ScheduleStatus is not null)
            {
                _logger.LogInformation($"Next timer schedule at: {myTimer.ScheduleStatus.Next}");
            }
            string fileName = $"EmailNotificationTimerLog_{dateTimeNow:yyyyMMdd_HHmmss}.txt";
            string filePath = Path.Combine(Path.GetTempPath(),fileName);
            string blobFileName =Path.GetFileName(filePath);
            string blobUri = Environment.GetEnvironmentVariable("BlobUri") ?? "https://default.blob.core.windows.net/";
            string containerName = Environment.GetEnvironmentVariable("StorageContainerName") ?? "mylogfiles";

            var exists = await _blobUtility.ContainerExistsAsync(containerName);
            
            if (!exists)
            {
                _logger.LogInformation($"Container '{containerName}' does not exist. Creating it now.");
                await _blobUtility.EnsureContainerAsync(containerName);
            }
            else
            {
                _logger.LogInformation($"Container '{containerName}' already exists.");
            }

            // Add a random log entry to the log file
            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                // Use Faker to generate  random Emplyee data
                Employee employee = new Employee
                {
                    UserName = _faker.Internet.UserName(),
                    Email = _faker.Internet.Email(),
                    CreatedOn = DateTime.Now
                };
                await writer.WriteLineAsync(JsonSerializer.Serialize(employee));
            }
            // Upload the log file to Azure Blob Storage
            byte[] fileBytes = await File.ReadAllBytesAsync(filePath);
            var blob = await _blobUtility.UploadBlobAsync(containerName, blobFileName, fileBytes);

            _logger.LogInformation($"Blob URI: {blobUri}, Container Name: {containerName}, File Name: {blobFileName}");
                        
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while executing the EmailNotificationTimerFunction.");
        }
    }

  

    private void LogAboutFunction(ILogger log)
    {
        FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(typeof(EmailNotificationTimer).Assembly.Location);
        var about = new Models.About
        {
            FunctionName = "EmailNotificationTimerFunction",
            Version = fileVersionInfo.FileVersion ?? "1.0.0",
            Status = "Active",
            InstanceName = Environment.GetEnvironmentVariable("WEBSITE_INSTANCE_ID") ?? "Unknown",
            Release_Version = fileVersionInfo.FileVersion ?? "1.0.0",
            Status_Last_Checked = DateTime.Now,

        };
        // Serialize the about object to JSON or any other format if needed

        log.LogInformation(JsonSerializer.Serialize(about));
    }
}
