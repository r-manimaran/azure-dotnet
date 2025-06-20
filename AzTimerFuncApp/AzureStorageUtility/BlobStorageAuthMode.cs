namespace AzureStorageUtility;

public enum BlobStorageAuthMode
{
    ManagedIdentity,
    UserAssignedManagedIdentity,
    ConnectionString,
    KeyVaultSasToken
}
