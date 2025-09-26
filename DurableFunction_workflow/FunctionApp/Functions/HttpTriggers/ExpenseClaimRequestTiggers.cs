using FunctionApp.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;

namespace FunctionApp.Functions.HttpTriggers;

public class ExpenseClaimRequestTiggers
{
    private readonly ILogger<ExpenseClaimRequestTiggers> _logger;

    public ExpenseClaimRequestTiggers(ILogger<ExpenseClaimRequestTiggers> logger)
    {
        _logger = logger;
    }

    [Function("SubmitExpenseClaimRequest")]
    public async Task<HttpResponseData> SubmitExpenseClaimRequest(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route ="expense/submit")] 
        HttpRequestData req,
        [DurableClient] DurableTaskClient client,
        FunctionContext context)
    {
        var logger = context.GetLogger("SubmitExpenseClaimRequest");
        try
        {
            var expenseRequest = await req.ReadFromJsonAsync<ExpenseRequest>();
            if (expenseRequest == null)
            {
                var badRequest = req.CreateResponse(System.Net.HttpStatusCode.BadRequest);
                await  badRequest.WriteAsJsonAsync(new { error = "Invalid expense request" });
                return badRequest;
            }

            var instanceId = await client.ScheduleNewOrchestrationInstanceAsync(nameof(ExpenseApprovalOrchestrator), expenseRequest);
            logger.LogInformation($"Started expense approval orchestration with ID = '{instanceId}'.");

            var response = req.CreateResponse(System.Net.HttpStatusCode.Accepted);
            await response.WriteAsJsonAsync(new { InstanceId=instanceId, RequestId = expenseRequest.RequestId });
            return response;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error submitting expense claim request");
            var response = req.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
            await response.WriteAsJsonAsync(new { error="Error submitting expense claim request" });
            return response;
        }
    }
}