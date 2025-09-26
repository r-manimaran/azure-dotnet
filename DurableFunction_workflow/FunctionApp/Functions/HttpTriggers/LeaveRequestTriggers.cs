using FunctionApp.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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

    [Function(nameof(GetRequestStatus))]
    public async Task<HttpResponseData> GetRequestStatus(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "request/{requestId}/status")] HttpRequestData req,
        string requestId,
        FunctionContext context)
    {
        var logger = context.GetLogger("GetRequestStatus");
        var dbContext = context.InstanceServices.GetRequiredService<AppDbContext>();

        var request = await dbContext.Requests.FirstOrDefaultAsync(x=>x.Id == requestId);

        if (request == null)
        {
            var notFound = req.CreateResponse(System.Net.HttpStatusCode.NotFound);
            await notFound.WriteAsJsonAsync(new { error = "Request not found" });
            return notFound;
        }
        var response = req.CreateResponse(System.Net.HttpStatusCode.OK);
        await response.WriteAsJsonAsync(new
        {
            RequestId = request.Id,
            Status = request.Status.ToString(),
            Type = request.Type.ToString(),
            CreatedAt = request.CreatedAt,
            UpdatedAt = request.UpdatedAt,
            Comments = request.Comments
        });
        return response;
    }

}