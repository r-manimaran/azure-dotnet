using FunctionApp.DTOs;
using FunctionApp.Models;
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
        var instanceId = context.InstanceId;
        logger.LogInformation("Expense approval orchestrator started for instance {instanceId}", instanceId);
        try
        {
            // Step 1: Validate expense request
            var validationResult = await context.CallActivityAsync<bool>(nameof(ValidationActivities.ValidateExpenseRequest), expenseRequest);

            if (!validationResult)
            {
                return "Expense request validation failed";
            }

            // Step 2: Persist in database
            await context.CallActivityAsync(nameof(DatabaseActivities.SaveRequest), new
            {
                Request = expenseRequest,
                Type = RequestType.Expense,
                InstanceId = instanceId
            });

            // setp 3: Update status to pending
            await context.CallActivityAsync(nameof(DatabaseActivities.UpdateRequestStatus), new
            {
                RequestId = expenseRequest.RequestId,
                Status = RequestStatus.Pending,
                Comments = "Validation successful. Pending approval."
            });

            // Step 4: Check if auto-approval applies ( eg. amount < $100)
            if (expenseRequest.TotalAmount < 100)
            {
                await context.CallActivityAsync(nameof(ExpenseActivities.AutoApproveExpense), expenseRequest);
                return "Expense request auto-approved";
            }

            // Step 5: Send notification to managerProcess
            await context.CallActivityAsync(nameof(NotificationActivities.SendExpenseManagerNotification), 
                new ExpenseManagerNotificationInput 
                {
                    EmployeeName = expenseRequest.EmployeeName,
                    ManagerEmail = expenseRequest.ManagerEmail,
                    Currency = expenseRequest.Currency,
                    TotalAmount = expenseRequest.TotalAmount,
                    InstanceId =  instanceId,
                    RequestId = expenseRequest.RequestId,
                    RequestedDate = expenseRequest.RequestedDate,
                    Items = ConvertToDTO(expenseRequest.Items)
                });
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
               
                // Step 6: Update status in database
                await context.CallActivityAsync(nameof(DatabaseActivities.UpdateRequestStatus), new
                {
                    RequestId = expenseRequest.RequestId,
                    Status = approvalResponse.Status == ApprovalStatus.Approved ? RequestStatus.Approved : RequestStatus.Rejected,
                    Comments = $"Expense request {approvalResponse.Status} by manager"
                });

                // Step 7: Email Employee on the Status
                await context.CallActivityAsync(nameof(NotificationActivities.SendEmpoyeeExpenseNotification),
                    new ExpenseEmployeeNotificationInput
                    {
                        EmployeeEmail = expenseRequest.EmployeeEmail,
                        EmployeeName = expenseRequest.EmployeeName,
                        Currency = expenseRequest.Currency,
                        Items   = ConvertToDTO(expenseRequest.Items),
                        RequestedDate = expenseRequest.RequestedDate,
                        TotalAmount = expenseRequest.TotalAmount,                        
                        Status = approvalResponse.Status == ApprovalStatus.Approved ? "Approved" : "Rejected",
                        Comments = $"Expense request {approvalResponse.Status} by manager"
                    });

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

    private static List<ExpenseItemDto> ConvertToDTO(List<ExpenseItem> items)
    {
        List<ExpenseItemDto> dtoItems = new List<ExpenseItemDto>();
        foreach (var item in items)
        {
            ExpenseItemDto dtoItem = new ExpenseItemDto();
            dtoItem.Date = item.Date;
            dtoItem.Category = item.Category.ToString();
            dtoItem.Description = item.Description;
            dtoItem.Amount = item.Amount;
            dtoItems.Add(dtoItem);
        }
        return dtoItems;
    }
}