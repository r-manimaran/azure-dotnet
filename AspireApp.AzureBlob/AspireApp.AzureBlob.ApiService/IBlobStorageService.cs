using AspireApp.AzureBlob.ApiService.Models;

namespace AspireApp.AzureBlob.ApiService;

public interface IBlobStorageService
{
    Task<bool> UploadBlobAsync(string containerName, string blobName, Stream content);
    Task<Stream> DownloadBlobAsync(string containerName, string blobName);
    Task<bool> DeleteBlobAsync(string containerName, string blobName);
    Task<string> GetBlob(string containerName, string blobName);
    Task<List<Blob>> GetAllBlobsWithUri(string containerName);

    Task<List<string>> GetAllBlobs(string containerName);
}
