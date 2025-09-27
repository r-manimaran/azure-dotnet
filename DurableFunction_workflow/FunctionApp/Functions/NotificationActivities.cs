using Microsoft.Azure.Functions.Worker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using FunctionApp.Services;
using FunctionApp.DTOs;

namespace FunctionApp.Functions;

public static class NotificationActivities
{
    [Function(nameof(SendManagerNotification))]
    public static async Task SendManagerNotification([ActivityTrigger] ManagerNotificationInput input, FunctionContext context)
    {
        var logger = context.GetLogger(nameof(SendManagerNotification));
        var emailService = context.InstanceServices.GetRequiredService<IEmailService>();
        await emailService.SendLeaveManagerApprovalEmailAsync(input);

        logger.LogInformation($"Manager notification sent for input:{JsonSerializer.Serialize(input)}");
    }

    [Function(nameof(SendEmployeeNotification))]
    public static async Task SendEmployeeNotification([ActivityTrigger] EmployeeNotificationInput input, FunctionContext context)
    {
        var logger = context.GetLogger(nameof(SendEmployeeNotification));
        var emailService = context.InstanceServices.GetRequiredService<IEmailService>();
        await emailService.SendEmployeeLeaveNotificationEmailAsync(input);

        logger.LogInformation($"Employee notification sent: {JsonSerializer.Serialize(input)}");
    }

    [Function(nameof(SendExpenseManagerNotification))]
    public static async Task SendExpenseManagerNotification([ActivityTrigger] ExpenseManagerNotificationInput input, FunctionContext context)
    {
        var logger = context.GetLogger(nameof(SendEmployeeNotification));
        var emailService = context.InstanceServices.GetRequiredService<IEmailService>();
        await emailService.SendExpenseManagerApprovalEmailAsync(input);
        logger.LogInformation($"Manager notification sent for input:{JsonSerializer.Serialize(input)}");
    }

    [Function(nameof(SendEmpoyeeExpenseNotification))]
    public static async Task SendEmpoyeeExpenseNotification([ActivityTrigger] ExpenseEmployeeNotificationInput input, FunctionContext context)
    {
        var logger = context.GetLogger(nameof(SendEmployeeNotification));
        var emailService = context.InstanceServices.GetRequiredService<IEmailService>();
        await emailService.SendExpenseEmployeeNotificationEmailAsync(input);
        logger.LogInformation($"Employee notification sent: {JsonSerializer.Serialize(input)}");
    }
}
