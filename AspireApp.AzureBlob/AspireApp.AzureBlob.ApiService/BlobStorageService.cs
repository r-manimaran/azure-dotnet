using AspireApp.AzureBlob.ApiService.Models;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;


namespace AspireApp.AzureBlob.ApiService;

public class BlobStorageService : IBlobStorageService
{
    private readonly BlobServiceClient _blobServiceClient;
    public BlobStorageService(BlobServiceClient blobServiceClient)
    {
        _blobServiceClient = blobServiceClient;
    }

    public async Task<bool> DeleteBlobAsync(string containerName, string blobName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        var blobClient = containerClient.GetBlobClient(blobName);

        return await blobClient.DeleteIfExistsAsync();
    }

    public async Task<Stream> DownloadBlobAsync(string containerName, string blobName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        var blobClient = containerClient.GetBlobClient(blobName);

        var response = await blobClient.DownloadStreamingAsync();
        return response.Value.Content;
    }

    public Task<List<string>> GetAllBlobs(string containerName)
    {
        throw new NotImplementedException();
    }

    public async Task<List<Blob>> GetAllBlobsWithUri(string containerName)
    {
        BlobContainerClient blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        var blobs = blobContainerClient.GetBlobsAsync();
        var blobList = new List<Blob>();

        string sasContainerSignature = string.Empty;
        if(blobContainerClient.CanGenerateSasUri)
        {
            BlobSasBuilder blobSasBuilder = new BlobSasBuilder()
            {
                BlobContainerName = blobContainerClient.Name,
                Resource = "c",//c- container,
                ExpiresOn = DateTimeOffset.Now.AddHours(5)
            };

            blobSasBuilder.SetPermissions(BlobAccountSasPermissions.Read);

            sasContainerSignature = blobContainerClient.GenerateSasUri(blobSasBuilder).AbsoluteUri.Split('?')[1].ToString();
        }

        await foreach(var blob in blobs)
        {
            var blobClient = blobContainerClient.GetBlobClient(blob.Name);
            Blob newBlob = new()
            {
                Uri = blobClient.Uri.AbsoluteUri
            };

            BlobProperties blobProperties = await blobClient.GetPropertiesAsync();
            if(blobProperties.Metadata.ContainsKey("title"))
            {
                newBlob.Title = blobProperties.Metadata["title"];
            }
            if(blobProperties.Metadata.ContainsKey("comment"))
            {
                newBlob.Comment = blobProperties.Metadata["comment"];
            }
            blobList.Add(newBlob);
        }
        return blobList;
    }

    public async Task<string> GetBlob(string containerName, string blobName)
    {
        BlobContainerClient blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        BlobClient blob = blobContainerClient.GetBlobClient(blobName);
        return blob.Uri.AbsoluteUri;
    }

    public async Task<bool> UploadBlobAsync(string containerName, string blobName, Stream content)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        await containerClient.CreateIfNotExistsAsync();

        var blobClient = containerClient.GetBlobClient(blobName);
        var result = await blobClient.UploadAsync(content, true);
        return result == null ? false : true; 
        
    }

   
}
