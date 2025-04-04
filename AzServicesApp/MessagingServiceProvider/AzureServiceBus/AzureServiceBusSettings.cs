using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessagingServiceProvider.AzureServiceBus;

public class AzureServiceBusSettings
{
    public const string SectionName = "AzureServiceBusSettings";

    public string ConnectionString { get; set; }
    public string TopicName { get; set; }
    public List<string> Subscriptions { get; set; } = new();
}
