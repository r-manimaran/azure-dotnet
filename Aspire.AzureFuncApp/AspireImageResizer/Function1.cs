using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace AspireImageResizer
{
    public class Function1
    {
        private readonly ILogger<Function1> _logger;
        private readonly BlobContainerClient _blobContainerClient;
        public Function1(ILogger<Function1> logger, BlobServiceClient client)
        {
            _logger = logger;
            _blobContainerClient = client.GetBlobContainerClient("uploads");
        }

    
        [Function("UploadImage")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function,"put",Route ="upload/{fileName}")] HttpRequest req, string fileName)
        {
            if(req.ContentType == "application/octet-stream")
            {
                try
                {
                    await _blobContainerClient.CreateIfNotExistsAsync();
                    await _blobContainerClient.UploadBlobAsync(fileName, req.Body);
                    _logger.LogInformation($"Uploaded {fileName}");
                    return new CreatedResult();
                }
                catch (Azure.RequestFailedException ex)  when (ex.ErrorCode == "BlobAlreadyExists")
                {
                    _logger.LogError($"Failed to upload {fileName} because it is already exists");
                    return new ConflictResult();
                }
            }
            return new NoContentResult();
        }
    }
}
