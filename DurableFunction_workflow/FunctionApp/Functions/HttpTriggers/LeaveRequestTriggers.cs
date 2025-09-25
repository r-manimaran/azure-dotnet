using FunctionApp.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace FunctionApp.Functions.HttpTriggers;

public class LeaveRequestTriggers
{
    private readonly ILogger<LeaveRequestTriggers> _logger;

    public LeaveRequestTriggers(ILogger<LeaveRequestTriggers> logger)
    {
        _logger = logger;
    }

    [Function("SubmitLeaveRequest")]
    public async Task<HttpResponseData> SubmitLeaveRequest(
        [HttpTrigger(AuthorizationLevel.Function,  "post", Route ="leave/submit")] HttpRequestData req,
        [DurableClient] DurableTaskClient client,
        FunctionContext context)
    {
        var logger = context.GetLogger("SubmitLeaveRequest");
        try
        {
            var leaveRequest = await req.ReadFromJsonAsync<LeaveRequest>();
            if (leaveRequest == null)
            {
                var badRequest = req.CreateResponse(System.Net.HttpStatusCode.BadRequest);
                await badRequest.WriteAsJsonAsync(new { error = "Invalid leave request" });
                return badRequest;
            }

            var instanceId = await client.ScheduleNewOrchestrationInstanceAsync(nameof(LeaveApprovalOrchestrator), leaveRequest);
            logger.LogInformation($"Started leave approval orchestration with ID = '{instanceId}'.");
            
            var response = req.CreateResponse(System.Net.HttpStatusCode.Accepted);
            await response.WriteAsJsonAsync(new { InstanceId = instanceId, RequestId=leaveRequest.RequestId });
            return response;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error submitting leave request");
            var response = req.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
            await response.WriteAsJsonAsync(new { error = "Error submitting leave request" });
            return response;
        }
    }
}