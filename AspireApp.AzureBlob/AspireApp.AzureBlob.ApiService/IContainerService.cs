namespace AspireApp.AzureBlob.ApiService;

public interface IContainerService
{
    Task<bool> CreateContainer(string containerName);
    Task<bool> DeleteContainer(string containerName);
    Task<ContainerBlobsResponse> GetAllContainersAndBlobs();
    Task<List<string>> GetAllContainer();
}