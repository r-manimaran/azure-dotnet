using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionApp.DTOs;

public class ExpenseRequest
{
    public string RequestId { get; set; } = Guid.NewGuid().ToString();
    public string EmployeeId { get; set; }
    public string EmployeeName { get; set; }
    public string EmployeeEmail { get; set; }
    public string ManagerId { get; set; }
    public string ManagerEmail { get; set; }
    public List<ExpenseItem> Items { get; set; } = new();
    public decimal TotalAmount { get; set; }
    public string Currency { get; set; } = "USD";
    public DateTime RequestedDate { get; set; } = DateTime.UtcNow;
    public ApprovalStatus ApprovalStatus { get; set; } = ApprovalStatus.Pending;
    public string? Comments { get; set; }
}

public class ExpenseItem
{
    public string Description { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public ExpenseCategory Category { get; set; }
    public string? Receipt { get; set; }
}
public enum ExpenseCategory
{
    Travel,
    Accommodation,
    Food,
    Office,
    Training,
    Transportation,
    Other
}   
