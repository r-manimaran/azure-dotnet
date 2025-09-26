using FunctionApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionApp.DTOs;

public class SaveRequestInput
{
    public object Request { get; set; }
    public RequestType Type { get; set; }
    public string InstanceId { get; set; }
}

public class UpdateStatusInput
{
    public string RequestId { get; set; }
    public RequestStatus Status { get; set; }
    public string? Comments { get; set; }
}