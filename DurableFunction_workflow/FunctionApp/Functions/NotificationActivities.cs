using Microsoft.Azure.Functions.Worker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace FunctionApp.Functions;

public static class NotificationActivities
{
    [Function(nameof(SendManagerNotification))]
    public static async Task SendManagerNotification([ActivityTrigger] object input, FunctionContext context)
    {
        var logger = context.GetLogger(nameof(SendManagerNotification));
        logger.LogInformation("Manager notification sent");
    }

    [Function(nameof(SendEmployeeNotification))]
    public static async Task SendEmployeeNotification([ActivityTrigger] object input, FunctionContext context)
    {
        var logger = context.GetLogger(nameof(SendEmployeeNotification));
        logger.LogInformation("Employee notification sent");
    }
}
