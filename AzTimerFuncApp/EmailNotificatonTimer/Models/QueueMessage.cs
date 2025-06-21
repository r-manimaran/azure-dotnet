using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Queues.Models;

namespace EmailNotificatonTimer.Models;

public class FileQueueMessage
{
    public string BlobFileUrl { get; set; } = string.Empty;
    public string BlobType { get; set; } = string.Empty;
    public string ContainerName { get; set; } = string.Empty;
    public string BlobFileName { get; set; } = string.Empty;
    public QueueMessage queueMessage { get; set; }
    public string BlobFileExtension { get; set; } = string.Empty;
}
