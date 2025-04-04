
namespace AzBlobFileUploader.Blob
{
    public interface IBlobService
    {
        Task DeleteAsync(string containerName, Guid fileId, CancellationToken cancellationToken = default);
        Task<FileResponse> DownloadAsync(string containerName, Guid fileId, CancellationToken cancellationToken = default);
        Task<IDictionary<string, string>> GetMetadataForBlob(string containerName, string blobName, CancellationToken cancellationToken = default);
        Task RemoveMetadataKeys(string containerName, string blobName, IEnumerable<string> keysToRemove, CancellationToken cancellationToken = default);
        Task SetMetadata(string containerName, string blobName, IDictionary<string, string> metadata = null, CancellationToken cancellationToken = default);
        Task UpdateMetadata(string containerName, string blobName, IDictionary<string, string> newMetadata, bool preserveExisting = true, CancellationToken cancellationToken = default);
        Task<string> UploadAsync(string containerName, Stream stream, string fileName, string contentType, IDictionary<string, string> metadata = null, CancellationToken cancellationToken = default);
    }
}