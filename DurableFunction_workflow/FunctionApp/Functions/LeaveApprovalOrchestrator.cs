using FunctionApp.DTOs;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;
using FunctionApp.Functions;

namespace FunctionApp.Functions;

public static class LeaveApprovalOrchestrator
{
    [Function(nameof(LeaveApprovalOrchestrator))]
    public static async Task<string> RunOrchestrator(
        [OrchestrationTrigger] TaskOrchestrationContext context)
    {
        ILogger logger = context.CreateReplaySafeLogger(nameof(LeaveApprovalOrchestrator));
        var leaveRequest = context.GetInput<LeaveRequest>();

        try
        {
            //step 1: Validate the leave request
            var validationResult = await context.CallActivityAsync<bool>(nameof(ValidationActivities.ValidateLeaveRequest), leaveRequest);

            if(!validationResult)
            {
                return "Leave request validation failed. Please check the request and try again.";
            }

            // Step 2: Send Notificaton to manager
            await context.CallActivityAsync(nameof(NotificationActivities.SendManagerNotification), new { Request = leaveRequest, Type = "Leave" });

            // Step 3: Wait for approval with timeout (7 days)
            using var cts = new CancellationTokenSource();
            var approvalTask = context.WaitForExternalEvent<ApprovalResponse>("ApprovalResponse");
            var timeoutTask = context.CreateTimer(context.CurrentUtcDateTime.AddDays(7), cts.Token);

            var winner = await Task.WhenAny(approvalTask, timeoutTask);
            if(winner == approvalTask)
            {
                cts.Cancel(); // Cancel the timer
                var approval = approvalTask.Result;

                // step 4: Process the approval
                await context.CallActivityAsync(nameof(LeaveApprovalActivities.ProcessLeaveApproval), new { Request = leaveRequest, Approval = approval });
                return $"Leave request {approval.Status}";
            }
            else
            {
                // step 5: Handle timeout - escalate to senior manager
                await context.CallActivityAsync(nameof(LeaveApprovalActivities.EscalateLeaveRequest), leaveRequest);
                return "Leave request timed out. Escalated to senior manager.";
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error validating leave request");
            await context.CallActivityAsync(nameof(LeaveApprovalActivities.HandleOrchestrationError),
                new { Request = leaveRequest, Error = ex.Message });
            return "Leave request validation failed. Please check the request and try again.";
        }
    }
    
}