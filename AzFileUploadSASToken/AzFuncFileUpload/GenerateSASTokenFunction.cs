using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Net;

namespace AzFuncFileUpload;

public class GenerateSASTokenFunction
{
    private readonly ILogger<GenerateSASTokenFunction> _logger;

    public GenerateSASTokenFunction(ILogger<GenerateSASTokenFunction> logger)
    {
        _logger = logger;
    }

    [Function("GenerateSasTokenFunction")]
    public IActionResult Run(
        [HttpTrigger(AuthorizationLevel.Function,  "post", Route ="api/files/generate-sas")] HttpRequest req)
    {
        _logger.LogInformation("Generating SAS TokenFunction triggered");

        try
        {
            // Get file name from the request body
            var fileName = req.Query["fileName"];
            if (string.IsNullOrEmpty(fileName))
            {
                _logger.LogError("File name is missing in the request.");
                return new BadRequestObjectResult("File name is required.");
            }

            // Set Storage account connection string and container name
            var storageConnectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
            if (string.IsNullOrEmpty(storageConnectionString))
            {
                _logger.LogError("Storage connection string is not set in the environment variables.");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
            var containerName = "myuploads"; // Replace with your container name

            // Create a BlobServiceClient to interact with the storage account
            var blobServiceClient = new BlobServiceClient(storageConnectionString);
            var blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);

            // Ensure the container exists
            // blobContainerClient.CreateIfNotExists();

            // Get a reference to the blob
            var blobName = $"uploads/{DateTime.UtcNow:yyyyMMdd}/{fileName}";
            string accountName = null;
            string accountKey = null;
            if (!string.IsNullOrEmpty(storageConnectionString))
            {
                var parts = storageConnectionString.Split(';');
                foreach (var part in parts)
                {
                    if (part.StartsWith("AccountName=", StringComparison.OrdinalIgnoreCase))
                        accountName = part.Substring("AccountName=".Length);
                    if (part.StartsWith("AccountKey=", StringComparison.OrdinalIgnoreCase))
                        accountKey = part.Substring("AccountKey=".Length);
                }
            }
            if (string.IsNullOrEmpty(accountName) || string.IsNullOrEmpty(accountKey))
            {
                _logger.LogError("Storage account name or key is missing in the connection string.");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
            // Generate a SAS token for the blob
            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = containerName,
                BlobName = blobName,
                Resource = "b", // 'b' for blob
                ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(15) // Set expiration time for the SAS token
            };
            // Grant Write permission to the SAS token
            sasBuilder.SetPermissions(BlobSasPermissions.Write);

            // sasBuilder.IPRange = new SasIPRange(IPAddress.Parse("192.168.0.1"), IPAddress.Parse("192.168.0.2"));
            // sasBuilder.Protocol = SasProtocol.Https; // Ensure the SAS token is only valid over HTTPS
            // Create the URL with the SAS token

            var sasToken = sasBuilder.ToSasQueryParameters(
    new StorageSharedKeyCredential(accountName, accountKey)).ToString();

            //var sasToken = sasBuilder.ToSasQueryParameters(new StorageSharedKeyCredential(
            //                blobServiceClient.AccountName,
            //                Environment.GetEnvironmentVariable("AzureWebJobsStorageKey"))).ToString();
           
            // SAS Url returned to the client as response
            var sasUrl = $"{blobContainerClient.Uri}/{blobName}?{sasToken}";
            _logger.LogInformation($"Generated SAS URL: {sasUrl}");
            return new OkObjectResult(new { SasUrl = sasUrl, Expiry = sasBuilder.ExpiresOn});

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while generating the SAS token.");
            //return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            return new  ObjectResult(new { error = "An error occurred while generating the SAS token." })
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
        }        
    }
}