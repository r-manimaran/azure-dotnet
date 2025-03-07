using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace AspireApp.AzureBlob.ApiService;

public class ContainerService : IContainerService
{
    private readonly BlobServiceClient _blobClient;

    public ContainerService(BlobServiceClient blobClient)
    {
        _blobClient = blobClient;
    }

    public async Task<bool> CreateContainer(string containerName)
    {
        BlobContainerClient blobContainerClient = _blobClient.GetBlobContainerClient(containerName);
        var result = await blobContainerClient.CreateIfNotExistsAsync(Azure.Storage.Blobs.Models.PublicAccessType.BlobContainer);
        return result == null ? false : true;
    }

    public async Task<bool> DeleteContainer(string containerName)
    {
        BlobContainerClient blobContainerClient = _blobClient.GetBlobContainerClient(containerName);
        return await blobContainerClient.DeleteIfExistsAsync();
    }

    public async Task<List<string>> GetAllContainer()
    {
        List<string> containerNames = new();
        await foreach(BlobContainerItem blobContainerItem in _blobClient.GetBlobContainersAsync())
        {
            containerNames.Add(blobContainerItem.Name);
        }
        return containerNames;
    }

    public async Task<ContainerBlobsResponse> GetAllContainersAndBlobs()
    {
        
        ContainerBlobsResponse response = new ContainerBlobsResponse();
        response.AccountName = _blobClient.AccountName; 
        await foreach(BlobContainerItem blobContainerItem in _blobClient.GetBlobContainersAsync())
        {
            
            Container container = new Container();
            container.Name = blobContainerItem.Name;

            BlobContainerClient blobContainer = _blobClient.GetBlobContainerClient(blobContainerItem.Name);
            int i = 0;
            await foreach(BlobItem item in blobContainer.GetBlobsAsync())
            {
                i++;
                BlobObject blobObject = new BlobObject();
                //get metadata information
                var blobClient = blobContainer.GetBlobClient(item.Name);
                BlobProperties properties = await blobClient.GetPropertiesAsync();
                string blobToAdd = item.Name;
                if (properties.Metadata.ContainsKey("title"))
                {
                    //containerAndBlobNames.Add($"Metadata Title:{properties.Metadata["title"]}");
                    blobObject.Title = properties.Metadata["title"];                    
                }
                blobObject.CreatedOn = properties.CreatedOn.ToString("dd-MMM-yyyy HH:mm");
                blobObject.Name = item.Name;
                container.Blobs.Add(blobObject);
            }
            response.Containers.Add(container);
           
        }
        return response;
    }
}
