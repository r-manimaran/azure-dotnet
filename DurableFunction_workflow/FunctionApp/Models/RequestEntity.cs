using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionApp.Models;

public class RequestEntity
{
    public string Id { get; set; }
    public string InstanceId { get; set; }
    public string EmployeeId { get; set; }
    public string EmployeeName { get; set; }
    public RequestType Type { get; set; }
    public string RequestData { get; set; }
    public RequestStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string? Comments { get; set; }
}

public enum RequestType
{
    Leave,
    Expense
}
public enum RequestStatus
{
    Submitted,
    Pending,
    Approved,
    Rejected,
    Escalated,
    Failed
}
