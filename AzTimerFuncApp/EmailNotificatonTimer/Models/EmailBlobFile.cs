using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailNotificatonTimer.Models;

public class EmailBlobFile
{
    public string FileName { get; set; }
    public string FileUrl { get; set; }
    public string ContentLength { get; set; }
    public string Host { get; set; }
    public string FileContent { get; set; }
    public string LastWriteTime { get; set; }
}
