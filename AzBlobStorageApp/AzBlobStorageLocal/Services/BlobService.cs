﻿
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace AzBlobStorageLocal.Services;

public class BlobService : IBlobService
{
    private readonly BlobServiceClient _blobServiceClient;
    private const string ContainerName = "files";

    public BlobService(BlobServiceClient blobServiceClient)
    {
        _blobServiceClient = blobServiceClient;
    }

    public async Task<Guid> UploadAsync(Stream stream, string contentType, CancellationToken cancellationToken = default)
    {
       BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(ContainerName);
       
       var fileId = Guid.NewGuid();
       BlobClient blobClient = containerClient.GetBlobClient(fileId.ToString());
       await blobClient.UploadAsync(
           stream,
           new BlobHttpHeaders { ContentType = contentType },
           cancellationToken:cancellationToken);
        return fileId;
    }

    public async Task DeleteAsync(Guid fileId, CancellationToken cancellationToken = default)
    {
        BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(ContainerName);
        BlobClient blobClient = containerClient.GetBlobClient(fileId.ToString());

        await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);
    }

    public async Task<FileResponse> DownloadAsync(Guid fileId, CancellationToken cancellationToken = default)
    {
        BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(ContainerName);
        BlobClient blobClient = containerClient.GetBlobClient(fileId.ToString());

        Response<BlobDownloadResult> response = await blobClient.DownloadContentAsync(cancellationToken: cancellationToken);
        return new FileResponse(response.Value.Content.ToStream(),
                                response.Value.Details.ContentType);
    }

    
}
