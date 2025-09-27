using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionApp.DTOs;

public class ExpenseManagerNotificationInput
{
    public string RequestId { get; set; }
    public string InstanceId { get; set; }
    public string EmployeeName { get; set; }
    public string ManagerEmail { get; set; }
    public decimal TotalAmount { get; set; }
    public string Currency { get; set; }
    public DateTime RequestedDate { get; set; }
    public List<ExpenseItemDto> Items { get; set; }
}

public class ExpenseEmployeeNotificationInput
{
    public string EmployeeName { get; set; }
    public string EmployeeEmail { get; set; }
    public string Status { get; set; }
    public string? Comments { get; set; }
    public decimal TotalAmount { get; set; }
    public string Currency { get; set; }
    public DateTime RequestedDate { get; set; }
    public List<ExpenseItemDto> Items { get; set; }

}

public class  ExpenseItemDto
{
    public string Description { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public string Category { get; set; }
}
