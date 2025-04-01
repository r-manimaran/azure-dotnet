using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Microsoft.Extensions.Options;

namespace AzStorageProfileApi.Services;

public class BlobStorageService : IBlobStorageService
{

    private readonly BlobServiceClient _blobServiceClient;

    private readonly IOptions<AzureBlobStorageSettings> _settings;

    public BlobStorageService(IOptions<AzureBlobStorageSettings> settings)
    {
        _settings = settings;

        _blobServiceClient = new BlobServiceClient(settings.Value.ConnectionString);    
       
    }

    public async Task<string> UploadFileAsync(IFormFile file, string containerName, string email)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        await containerClient.CreateIfNotExistsAsync();

        var blobName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var blobClient = containerClient.GetBlobClient(blobName);

        // create metadata dictionary
        var metadata = new Dictionary<string, string>
        {
            { "useremail",email},
            { "uploaddate",DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") },
            { "originalfilename",file.FileName}
        };

        // set blob upload options with metadata
        var blobUploadOptions = new BlobUploadOptions
        {
            Metadata = metadata
        };

        using (var stream = file.OpenReadStream())
        {
            await blobClient.UploadAsync(stream, blobUploadOptions);
        }

        return blobClient.Uri.ToString();
    }

    public string GenerateSasToken(string blobUrl, string containerName, TimeSpan expiryTime)
    {
        var blobUri = new Uri(blobUrl);
        var blobName = blobUri.Segments.Last();

        var sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = containerName,
            BlobName = blobName,
            Resource = "b",
            StartsOn = DateTimeOffset.UtcNow,
            ExpiresOn = DateTimeOffset.UtcNow.Add(expiryTime)
        };

        sasBuilder.SetPermissions(BlobSasPermissions.Read);

        var sasToken = sasBuilder.ToSasQueryParameters(new StorageSharedKeyCredential(_settings.Value.AccountName, _settings.Value.AccountKey)).ToString();

        return $"{blobUrl}?{sasToken}";
    }

    public async Task<IDictionary<string, string>> GetBlobMetadataAsync(string blobUrl, string containerName)
    {
        var blobUri = new Uri(blobUrl);
        var blobName = blobUri.Segments.Last();

        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        var blobClient = containerClient.GetBlobClient(blobName);

        var properties = await blobClient.GetPropertiesAsync();
        return properties.Value.Metadata;
    }

    public async Task UpdateBlobMetadataAsync(string blobUrl, string containerName, IDictionary<string, string> metadata)
    {
        var blobUri = new Uri(blobUrl);
        var blobName = blobUri.Segments.Last();

        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        var blobClient = containerClient.GetBlobClient(blobName);

        await blobClient.SetMetadataAsync(metadata);
    }

    public async Task<string> GenerateUserDelegationSasToken(string containerName, string blobName)
    {
        var BlobServiceClient = new BlobServiceClient(_settings.Value.ConnectionString);

        // Get User delegation key
        var userDelegationKey = await BlobServiceClient.GetUserDelegationKeyAsync(DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddHours(1));

        // Generate SAS token with specific permission
        BlobSasBuilder sasBuilder = new BlobSasBuilder()
        {
            BlobContainerName = containerName,
            BlobName = blobName,
            Resource = "b",
            StartsOn = DateTimeOffset.UtcNow,
            ExpiresOn = DateTimeOffset.UtcNow.AddHours(1)
        };

        sasBuilder.SetPermissions(BlobSasPermissions.Read);

        // Generate the SAS token
        var sasToken = sasBuilder.ToSasQueryParameters(userDelegationKey, BlobServiceClient.AccountName).ToString();
        return  sasToken;
    }
}
