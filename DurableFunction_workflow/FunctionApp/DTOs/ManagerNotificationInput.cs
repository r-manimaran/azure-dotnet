using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionApp.DTOs;

public class ManagerNotificationInput
{
    public string RequestId { get; set; }
    public string InstanceId { get; set; }
    public string EmployeeName { get; set; }
    public string ManagerEmail { get; set; }
    public string RequestType { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TotalDays { get; set; }
    public string Reason { get; set; }
}

public class EmployeeNotificationInput
{
    public string EmployeeName { get; set; }
    public string EmployeeEmail { get; set; }
    public string RequestType { get; set; }
    public string Status { get; set; }
    public string? Comments { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}
