using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzFileProcessing.Common;

public class IngestionReceived : Event
{
    public string BlobUrl { get; set; }
}
