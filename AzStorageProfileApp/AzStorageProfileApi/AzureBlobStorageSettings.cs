namespace AzStorageProfileApi;

public class AzureBlobStorageSettings
{
    public const string SectionName = "AzureBlobStorageSettings";
    public string ConnectionString { get; set; }
    public string AccountName { get; set; }
    public string AccountKey { get; set; }
    public string ProfileImageContainer { get; set; }
}
