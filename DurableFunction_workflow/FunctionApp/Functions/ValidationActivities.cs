using FunctionApp.DTOs;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionApp.Functions;

public static class ValidationActivities
{
    [Function(nameof(ValidateLeaveRequest))]
    public static bool ValidateLeaveRequest([ActivityTrigger] LeaveRequest request, FunctionContext content)
    {
        var logger = content.GetLogger(nameof(ValidateLeaveRequest));
        logger.LogInformation($"ValidationActivities: {request}");

        // Business validation logic
        if(request.StartDate < DateTime.Today)
        {
            logger.LogWarning("Leave start date cannot be in the past");
            return false;
        }
        if (request.EndDate <= request.StartDate)
        {
            logger.LogWarning("Leave end date must be after start date");
            return false;
        }
        if(request.TotalDays >30)
        {
            logger.LogWarning("Leave duration cannot be more than 30 days");
            return false;
        }
        return true;
    }

    [Function(nameof(ValidateExpenseRequest))]
    public static bool ValidateExpenseRequest([ActivityTrigger] ExpenseRequest request, FunctionContext context)
    {
        var logger = context.GetLogger(nameof(ValidateExpenseRequest));

        if(request.TotalAmount <= 0)
        {
            logger.LogWarning("Expense amount must be greater than 0");
            return false;
        }
        if (request.Items == null || !request.Items.Any())
        {
            logger.LogWarning("Expense request must have  at least one item");
            return false;
        }
        return true;
    }
}
