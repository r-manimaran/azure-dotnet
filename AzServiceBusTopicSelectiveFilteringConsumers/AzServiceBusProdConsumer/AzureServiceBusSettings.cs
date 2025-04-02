namespace AzServiceBusProdConsumer;

public class AzureServiceBusSettings
{
    public const string SectionName = "AzureServiceBusSettings";

    public string ConnectionString { get; set; }
    public string TopicName { get; set; }
    public List<string> Subscriptions { get; set; } = new();
}
