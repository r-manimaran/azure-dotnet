using FunctionApp.DTOs;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;

namespace FunctionApp.Functions;

public static class ExpenseApprovalOrchestrator
{
    [Function(nameof(ExpenseApprovalOrchestrator))]
    public static async Task<string> RunOrchestrator(
        [OrchestrationTrigger] TaskOrchestrationContext context)
    {
        ILogger logger = context.CreateReplaySafeLogger(nameof(ExpenseApprovalOrchestrator));
        var expenseRequest = context.GetInput<ExpenseRequest>();

        try
        {
            // Step 1: Validate expense request
            var validationResult = await context.CallActivityAsync<bool>(nameof(ValidationActivities.ValidateExpenseRequest), expenseRequest);

            if (!validationResult)
            {
                return "Expense request validation failed";
            }

            // Step 2: Check if auto-approval applies ( eg. amount < $100)
            if(expenseRequest.TotalAmount < 100)
            {
                await context.CallActivityAsync(nameof(ExpenseActivities.AutoApproveExpense), expenseRequest);
                return "Expense request auto-approved";
            }

            // Step 3: Send notification to managerProcess
            await context.CallActivityAsync(nameof(NotificationActivities.SendManagerNotification), 
                new { Request = expenseRequest, Type="Expense" });
            logger.LogInformation("Notification sent to manager");

            // Step 4: Wait for manager approval with Timeout of 5 days
            using var cts = new CancellationTokenSource();
            var approvalTask = context.WaitForExternalEvent<ApprovalResponse>("ApprovalResponse");
            var timeoutTask = context.CreateTimer(context.CurrentUtcDateTime.AddDays(5), cts.Token);
            var winner = await Task.WhenAny(approvalTask, timeoutTask);
            if(winner == approvalTask)
            {
                cts.Cancel(); // cancel the timer task
                var approvalResponse = approvalTask.Result;

                // Step 5: Process manager approval decision
                await context.CallActivityAsync(nameof(ExpenseActivities.ProcessExpenseApproval), new { Request = expenseRequest, Approval = approvalResponse });
                return $"Expense request {approvalResponse.Status} by manager";
            }
            else
            {
                // Step 6: Handle timeout
                await context.CallActivityAsync(nameof(ExpenseActivities.EscalateExpenseRequest), expenseRequest);
                return "Expense request escalated due to timeout";
            }               

        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in expense approval orchestrator");
            await context.CallActivityAsync(nameof(ExpenseActivities.EscalateExpenseRequest), new { Request = expenseRequest, Error = ex.Message });
            return "Error in expense approval orchestrator";
        }
    }

   

    
}