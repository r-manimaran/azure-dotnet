using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzFileProcessing.Common;

public class ProcessFileEvent : Event
{
    public string BlobUrl { get; set; }
    public string FileName { get; set; }
    public string FileExtension { get; set; }
}
