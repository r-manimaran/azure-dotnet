using Azure.Storage.Queues.Models;

namespace AzBlobFileUploader.Models;

public class FileProcessingQueueMessage
{
    public string BlobFileUrl { get; set; }
    public string BlobType { get; set; }
    public string ContainerName { get; set; }
    public string BlobFileName { get; set; }
    public QueueMessage QueueMessage { get; set; }
    public string  BlobFileExtension { get; set; }
}
