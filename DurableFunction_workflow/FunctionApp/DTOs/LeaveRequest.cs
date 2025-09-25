using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionApp.DTOs;

public class LeaveRequest
{
    public string RequestId { get; set; } = Guid.NewGuid().ToString();
    public string EmployeeId { get; set; }
    public string EmployeeName { get; set; }
    public string EmployeeEmail { get; set; }
    public string ManagerId { get; set; }
    public string ManagerEmail { get; set; }
    public LeaveType LeaveType { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TotalDays { get; set; }
    public string Reason { get; set; }
    public DateTime RequestedDate { get; set; } = DateTime.UtcNow;
    public ApprovalStatus ApprovalStatus { get; set; } = ApprovalStatus.Pending;
    public string? Comments { get; set; }
}

public enum LeaveType
{
    Sick,
    Vacation,
    Personal,
    Maternity,
    Paternity,
    VisaRenewal,
    Emergency,
    Unpaid,
    Furlough,
    Other
}
