namespace AspireApp.AzureBlob.ApiService;

public class ContainerBlobsResponse
{
    public string AccountName { get; set; }
    public List<Container> Containers { get; set; } = new();
}


public class Container
{
    public string Name { get; set; }
    public List<BlobObject> Blobs { get; set; } = new();
}

public class BlobObject
{
    public string Name { get; set; }
    public string Title { get; set; }
    public string CreatedOn { get; set; }
}