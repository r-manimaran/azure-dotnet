using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailNotificatonTimer.Models;

public class About
{
    public string FunctionName { get; set; } = "EmailNotificationTimerFunction";
    public string Version { get; set; } = "1.0.0";
    public string Status { get; set; } = "Active";
    public string InstanceName { get; set; }= Environment.GetEnvironmentVariable("WEBSITE_INSTANCE_ID") ?? "Unknown";
    public string Release_Version  { get; set; }
    public DateTime Status_Last_Checked { get; set; } 
}
