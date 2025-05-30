﻿
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace AzBlobStorageLocal.Services;

public class BlobService : IBlobService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly ILogger _logger;
    private const string ContainerName = "files";

    public BlobService(BlobServiceClient blobServiceClient, ILogger<BlobService> logger)
    {
        _blobServiceClient = blobServiceClient;
        _logger = logger;
    }

    public async Task<Guid> UploadAsync(Stream stream, string contentType, CancellationToken cancellationToken = default)
    {
       BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(ContainerName);
       
       var fileId = Guid.NewGuid();
       _logger.LogInformation("Upload to Azure with {fileId}",fileId);
       BlobClient blobClient = containerClient.GetBlobClient(fileId.ToString());
       await blobClient.UploadAsync(
           stream,
           new BlobHttpHeaders { ContentType = contentType },
           cancellationToken:cancellationToken);
        _logger.LogInformation("Uploaded the file successfully.");
        return fileId;
    }

    public async Task DeleteAsync(Guid fileId, CancellationToken cancellationToken = default)
    {
        BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(ContainerName);
        BlobClient blobClient = containerClient.GetBlobClient(fileId.ToString());
        _logger.LogInformation("Deleting the file with fileId- {fileId}",fileId.ToString());

        await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);
        
        _logger.LogInformation("Deleted blob File with fileId {fileId}",fileId.ToString());
    }

    public async Task<FileResponse> DownloadAsync(Guid fileId, CancellationToken cancellationToken = default)
    {
        BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(ContainerName);
        _logger.LogInformation("Downloading file with fileId-{fileId}", fileId);
        BlobClient blobClient = containerClient.GetBlobClient(fileId.ToString());

        Response<BlobDownloadResult> response = await blobClient.DownloadContentAsync(cancellationToken: cancellationToken);
        return new FileResponse(response.Value.Content.ToStream(),
                                response.Value.Details.ContentType);
    }

    
}
