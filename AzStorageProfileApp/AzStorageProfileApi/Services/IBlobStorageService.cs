
namespace AzStorageProfileApi.Services
{
    public interface IBlobStorageService
    {
        string GenerateSasToken(string blobUrl, string containerName, TimeSpan expiryTime);
        Task<string> UploadFileAsync(IFormFile file, string containerName, string email);

        Task<IDictionary<string, string>> GetBlobMetadataAsync(string blobUrl, string containerName);
        Task UpdateBlobMetadataAsync(string blobUrl, string containerName, IDictionary<string, string> metadata);
    }
}