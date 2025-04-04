namespace FileIngestionProcessor;

public class AzureSetting
{
    public const string SectionName = "AzureSetting";

    public string StorageQueueName { get; set; }
    public string ServiceBusTopicName { get; set; }
    public int StorageQueueMessageCountToReceive { get; set; }
    public int VisibilityTimeout { get; set; }
}
