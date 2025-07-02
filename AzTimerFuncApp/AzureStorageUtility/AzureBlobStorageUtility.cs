using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureStorageUtility;

public class AzureBlobStorageUtility
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly ILogger<AzureBlobStorageUtility> _logger;
    public AzureBlobStorageUtility(
            string storageAccountUrlOrConnectionString, 
            BlobStorageAuthMode authMode,
            string? keyVaultUri=null, 
            string? secretName=null, 
            string? clientId=null,
            ILogger<AzureBlobStorageUtility> logger = null)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _blobServiceClient = authMode switch
        {
            BlobStorageAuthMode.ManagedIdentity => new BlobServiceClient(new Uri(storageAccountUrlOrConnectionString)),
            BlobStorageAuthMode.UserAssignedManagedIdentity => new BlobServiceClient(new Uri(storageAccountUrlOrConnectionString), new DefaultAzureCredential(new DefaultAzureCredentialOptions { ManagedIdentityClientId = clientId })),
            BlobStorageAuthMode.ConnectionString => new BlobServiceClient(storageAccountUrlOrConnectionString),
            BlobStorageAuthMode.KeyVaultSasToken => GetClientUsingSasFromKeyVault(storageAccountUrlOrConnectionString, keyVaultUri, secretName).GetAwaiter().GetResult(),
            _ => throw new ArgumentException("Invalid authentication mode specified.")
        };
        _logger?.LogInformation("AzureBlobStorageUtility initialized with authentication mode: {AuthMode}", authMode);  
    }

    /// <summary>
    /// Creates a <see cref="BlobServiceClient"/> instance using a SAS token retrieved from Azure Key Vault.
    /// </summary>
    /// <remarks>This method retrieves a SAS token stored as a secret in Azure Key Vault and uses it to create
    /// a <see cref="BlobServiceClient"/>. Ensure that the provided Key Vault URI and secret name are valid and
    /// accessible.</remarks>
    /// <param name="storageAccountUrlOrConnectionString">The URL or connection string of the Azure Storage account. This must include the base URL of the storage
    /// account.</param>
    /// <param name="keyVaultUri">The URI of the Azure Key Vault from which the SAS token will be retrieved. Cannot be <see langword="null"/>.</param>
    /// <param name="secretName">The name of the secret in Azure Key Vault that contains the SAS token. Cannot be <see langword="null"/>.</param>
    /// <returns>A <see cref="BlobServiceClient"/> instance configured with the SAS token retrieved from Azure Key Vault.</returns>
    private async Task<BlobServiceClient> GetClientUsingSasFromKeyVault(string storageAccountUrlOrConnectionString, string? keyVaultUri, string? secretName)
    {
        var secretClient = new SecretClient(new Uri(keyVaultUri), new DefaultAzureCredential());
        var secret = await secretClient.GetSecretAsync(secretName);
        var sasToken = secret.Value.Value;
        return new BlobServiceClient(new Uri($"https://samaranaspire.blob.core.windows.net/mylogfiles?{sasToken}"));
       // return new BlobServiceClient($"{storageAccountUrlOrConnectionString}?{sasToken}");
    }

    #region Container operations
    public async Task EnsureContainerAsync(string containerName)
    {
        _logger?.LogInformation("Ensuring container: {ContainerName}", containerName);
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        await containerClient.CreateIfNotExistsAsync();
    }
    public async Task<bool> ContainerExistsAsync(string containerName)
    {
        _logger?.LogInformation("Checking if container exists: {ContainerName}", containerName);
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        return await containerClient.ExistsAsync();
    }
    public async Task<List<string>> GetContainerListAsync()
    {
        _logger?.LogInformation("Retrieving list of containers");
        var containers = _blobServiceClient.GetBlobContainersAsync();
        var containerList = new List<string>();
        await foreach (var container in containers)
        {
            containerList.Add(container.Name);
        }
        return containerList;
    }

    public async Task DeleteContainerAsync(string containerName)
    {
        _logger?.LogInformation("Deleting container: {ContainerName}", containerName);
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        await containerClient.DeleteIfExistsAsync();
    }
    #endregion

    #region Blob operations
    public async Task<bool> CheckBlobExistsAsync(string containerName, string blobName)
    {
        _logger?.LogInformation("Checking if blob exists: {BlobName} in container: {ContainerName}", blobName, containerName);
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        var blobClient = containerClient.GetBlobClient(blobName);
        return await blobClient.ExistsAsync();
    }
    public async Task<List<string>> GetBlobListAsync(string containerName)
    {
        _logger?.LogInformation("Retrieving list of blobs in container: {ContainerName}", containerName);
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        var blobs = containerClient.GetBlobsAsync();
        var blobList = new List<string>();

        await foreach (var blob in blobs)
        {
            blobList.Add(blob.Name);
        }
        return blobList;
    }

    public async Task<string> UploadBlobAsync(string containerName, string blobName, byte[] content)
    {
        _logger?.LogInformation("Uploading blob: {BlobName} to container: {ContainerName}", blobName, containerName);
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        await containerClient.CreateIfNotExistsAsync();

        var blobClient = containerClient.GetBlobClient(blobName);
        using var stream = new MemoryStream(content);
        // Retry logic can be added
        await RetryAsync(async () =>
        {
            await blobClient.UploadAsync(stream, overwrite: true);
        });
        
        return blobClient.Uri.ToString();
    }

    public async Task<byte[]> DownloadBlobAsync(string containerName, string blobName)
    {
        _logger?.LogInformation("Downloading blob: {BlobName} from container: {ContainerName}", blobName, containerName);
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        var blobClient = containerClient.GetBlobClient(blobName);

        return await RetryAsync(async () =>
        {
            var response = await blobClient.DownloadAsync();
            using var memoryStream = new MemoryStream();
            await response.Value.Content.CopyToAsync(memoryStream);
            return memoryStream.ToArray();
        }); 
        
    }

    public async Task DeleteBlobAsync(string containerName, string blobName)
    {
        _logger?.LogInformation("Deleting blob: {BlobName} from container: {ContainerName}", blobName, containerName);
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        var blobClient = containerClient.GetBlobClient(blobName);
        await blobClient.DeleteIfExistsAsync();
    }
    #endregion

    #region Retry Logic
    private async Task RetryAsync(Func<Task> action, int maxRetries = 3, int delayMilliseconds = 1000)
    {
        for (int i = 0; i < maxRetries; i++)
        {
            _logger?.LogInformation("Attempt {Attempt} of {MaxRetries}", i + 1, maxRetries);
            try
            {
                await action();
                return;
            }
            catch (Exception ex) when (i < maxRetries - 1)
            {
                await Task.Delay(delayMilliseconds);
            }
        }
        throw new InvalidOperationException("Operation failed after maximum retries.");
    }
    private async Task<T> RetryAsync<T>(Func<Task<T>> action, int maxRetries = 3, int delayMilliseconds = 1000)
    {
        for (int i = 0; i < maxRetries; i++)
        {
            _logger?.LogInformation("Attempt {Attempt} of {MaxRetries}", i + 1, maxRetries);
            try
            {
                return await action();
            }
            catch (Exception ex) when (i < maxRetries - 1)
            {
                await Task.Delay(delayMilliseconds);
            }
        }
        throw new InvalidOperationException("Operation failed after maximum retries.");
    }
    #endregion

    #region Event Grid Integration
    
    #endregion
}
