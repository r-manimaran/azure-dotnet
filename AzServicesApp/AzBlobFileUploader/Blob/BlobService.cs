
using Azure;
using Azure.Core;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Options;

namespace AzBlobFileUploader.Blob;

public class BlobService : IBlobService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly IOptions<AzureBlobSettings> _options;
    private readonly ILogger<BlobService> _logger;
    public BlobService(BlobServiceClient blobServiceClient,
                       IOptions<AzureBlobSettings> options,
                       ILogger<BlobService> logger)
    {
        _blobServiceClient = blobServiceClient;
        _options = options;
        _logger = logger;
    }

    public async Task<string> UploadAsync(string containerName, Stream stream, string fileName, string contentType,
                                          IDictionary<string, string> metadata = null, CancellationToken cancellationToken = default)
    {
        BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(containerName);

        // var fileId = Guid.NewGuid();
        _logger.LogInformation("Upload to Azure with filename: {filename}", fileName);

        BlobClient blobClient = containerClient.GetBlobClient(fileName);

        // Check if we have any metadata to be associated

        var options = new BlobUploadOptions
        {
            HttpHeaders = new BlobHttpHeaders
            {
                ContentType = contentType
            },
            Metadata = metadata
        };

        await blobClient.UploadAsync(
            stream,
            options,
            cancellationToken: cancellationToken);

        _logger.LogInformation("File Uploaded successfully.");

        _logger.LogInformation("File Url: {fileUrl}", blobClient.Uri.AbsoluteUri);

        return blobClient.Uri.AbsoluteUri;
    }

    /// <summary>
    /// Delete Blob
    /// </summary>
    /// <param name="containerName"></param>
    /// <param name="fileId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task DeleteAsync(string containerName, Guid fileId, CancellationToken cancellationToken = default)
    {
        BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(containerName);

        BlobClient blobClient = containerClient.GetBlobClient(fileId.ToString());

        _logger.LogInformation("Deleting the file with fileId- {fileId}", fileId.ToString());

        await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);

        _logger.LogInformation("Deleted blob File with fileId {fileId}", fileId.ToString());
    }

    /// <summary>
    /// Download Blob File
    /// </summary>
    /// <param name="containerName"></param>
    /// <param name="fileId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<FileResponse> DownloadAsync(string containerName, Guid fileId, CancellationToken cancellationToken = default)
    {
        BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(containerName);

        _logger.LogInformation("Downloading file with fileId-{fileId}", fileId);

        BlobClient blobClient = containerClient.GetBlobClient(fileId.ToString());

        Response<BlobDownloadResult> response = await blobClient.DownloadContentAsync(cancellationToken: cancellationToken);

        return new FileResponse(response.Value.Content.ToStream(),
                                response.Value.Details.ContentType);
    }

    public async Task<IDictionary<string, string>> GetMetadataForBlob(string containerName, string blobName, CancellationToken cancellationToken = default)
    {
        BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(containerName);

        BlobClient blobClient = containerClient.GetBlobClient(blobName);

        var properties = await blobClient.GetPropertiesAsync(cancellationToken: cancellationToken);

        return properties.Value.Metadata;
    }

    public async Task SetMetadata(string containerName, string blobName, IDictionary<string, string> metadata = null, CancellationToken cancellationToken = default)
    {
        BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(containerName);

        BlobClient blobClient = containerClient.GetBlobClient(blobName);

        await blobClient.SetMetadataAsync(metadata, cancellationToken: cancellationToken);
    }

    public async Task UpdateMetadata(string containerName, string blobName, IDictionary<string, string> newMetadata,
                                     bool preserveExisting = true, CancellationToken cancellationToken = default)
    {
        BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(containerName);

        BlobClient blobClient = containerClient.GetBlobClient(blobName);

        // Get existing metadata if we want to preserve it
        IDictionary<string, string> finalMetadata = new Dictionary<string, string>();

        if (preserveExisting)
        {
            // Get the existing metadata
            var properties = await blobClient.GetPropertiesAsync(cancellationToken: cancellationToken);

            foreach (var item in properties.Value.Metadata)
            {
                finalMetadata[item.Key] = item.Value;
            }
        }

        //Add or update new metadata
        foreach (var item in newMetadata)
        {
            finalMetadata[item.Key] = item.Value;
        }

        //set the combined metadata back to the blob
        await blobClient.SetMetadataAsync(finalMetadata, cancellationToken: cancellationToken);

        _logger.LogInformation("Metadata updated successfully for blob: {blobName}", blobName);
    }

    public async Task RemoveMetadataKeys(string containerName, string blobName, IEnumerable<string> keysToRemove, CancellationToken cancellationToken = default)
    {
        BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(containerName);

        BlobClient blobClient = containerClient.GetBlobClient(blobName);

        // Get the existing Metaata
        var properties = await blobClient.GetPropertiesAsync(cancellationToken: cancellationToken);
        var metadata = properties.Value.Metadata.ToDictionary(k => k.Key, v => v.Value);

        // Remove specified keys
        foreach (var key in keysToRemove)
        {
            metadata.Remove(key);
        }

        // Update blob with remaining metadata
        await blobClient.SetMetadataAsync(metadata, cancellationToken: cancellationToken);

        _logger.LogInformation("Metadata keys removed successfully for blob: {blobName}", blobName);
    }

}
