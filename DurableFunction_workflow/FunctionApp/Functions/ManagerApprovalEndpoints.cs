using Castle.Core.Logging;
using FunctionApp.DTOs;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionApp.Functions;

public class ManagerApprovalEndpoints
{
    private readonly ILogger<ManagerApprovalEndpoints> _logger;

    public ManagerApprovalEndpoints(ILogger<ManagerApprovalEndpoints> logger)
    {
        _logger = logger;
    }


    [Function("ApproveLeaveRequest")]
    public async Task<HttpResponseData> ApproveLeaveRequest(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "ManagerApproval/ApproveLeave/{instanceId}")] HttpRequestData req,
        string instanceId,
        [DurableClient] DurableTaskClient client)
    {
        // Implementation for approving a leave request
        _logger.LogInformation($"Manager approved leave request with instance ID: {instanceId}");

        // check the orchestration status 
        var status = await client.GetInstanceAsync(instanceId);
        if(status == null){
            var notFoundResponse = req.CreateResponse(System.Net.HttpStatusCode.NotFound);
            await notFoundResponse.WriteStringAsync(string.Format("Orchestration not found with {InstanceId}",instanceId));
            return notFoundResponse;
        }
        if(status.RuntimeStatus !=OrchestrationRuntimeStatus.Running && 
            status.RuntimeStatus != OrchestrationRuntimeStatus.Pending)
        {
            var conflictResponse = req.CreateResponse(System.Net.HttpStatusCode.Conflict);
            await conflictResponse.WriteStringAsync(string.Format("Orchestration is not running or pending with {InstanceId}", instanceId));
            return conflictResponse;
        }
        var approvalDto = new ApprovalResponse
        {
            InstanceId = instanceId,
            Status = ApprovalStatus.Approved,
            ApprovalFor = ApprovalFor.Leave
        };

        await client.RaiseEventAsync(instanceId, "ApprovalResponse", approvalDto);
        _logger.LogInformation($"ApprovalResponse event raised for instance ID: {instanceId}");

        var response = req.CreateResponse(System.Net.HttpStatusCode.OK);
        await response.WriteStringAsync(string.Format("Leave request approved with {InstanceId}", instanceId));
        return response;
    }

    [Function("RejectLeaveRequest")]
    public async Task<HttpResponseData> RejectLeaveRequest(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "ManagerApproval/RejectLeave/{instanceId}")] HttpRequestData req,
        string instanceId,
        [DurableClient] DurableTaskClient client)
    {
        // Implementation for rejecting a leave request
        _logger.LogInformation($"Manager rejected leave request with instance ID: {instanceId}");

        // Check the orchestration status
        var status = await client.GetInstanceAsync(instanceId);

        if(status == null)
        {
            var notFoundResponse = req.CreateResponse(System.Net.HttpStatusCode.NotFound);
            await notFoundResponse.WriteStringAsync(string.Format("Orchestration not found with {InstanceId}", instanceId));
            return notFoundResponse;
        }

        if(status.RuntimeStatus != OrchestrationRuntimeStatus.Running && status.RuntimeStatus != OrchestrationRuntimeStatus.Pending)
        {
            var conflictResponse = req.CreateResponse(System.Net.HttpStatusCode.Conflict);
            await conflictResponse.WriteStringAsync(string.Format("Orchestration is not running or pending with {InstanceId}", instanceId));
            return conflictResponse;
        }
        var approvalDto = new ApprovalResponse
        {
            InstanceId = instanceId,
            Status = ApprovalStatus.Rejected,
            ApprovalFor = ApprovalFor.Leave
        };

        await client.RaiseEventAsync(instanceId, "ApprovalResponse", approvalDto);
        _logger.LogInformation($"ApprovalResponse event raised for instance ID: {instanceId}");

        var response =req.CreateResponse(System.Net.HttpStatusCode.OK);
        await response.WriteStringAsync(string.Format("Leave request rejected with {InstanceId}", instanceId));
        return response;

    }

    [Function("ApproveExpenseClaim")]
    public async Task<HttpResponseData> ApproveExpenseClaim(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "ManagerApproval/expenseApprove/{instanceId}")] HttpRequestData req,
        string instanceId,
        [DurableClient] DurableTaskClient client)
    {
        // Implementation for approving an expense claim
        _logger.LogInformation($"Manager approved expense claim with instance ID: {instanceId}");

        // Get the status of the orchestration
        var status = await client.GetInstanceAsync(instanceId);
        if (status == null)
        {
            var notFoundResponse = req.CreateResponse(System.Net.HttpStatusCode.NotFound);
            await notFoundResponse.WriteStringAsync(string.Format("Orchestration not found with {InstanceId}", instanceId));
            return notFoundResponse;
        }
        if(status.RuntimeStatus != OrchestrationRuntimeStatus.Running && 
            status.RuntimeStatus != OrchestrationRuntimeStatus.Pending)
        {
            var conflictResponse = req.CreateResponse(System.Net.HttpStatusCode.Conflict);
            await conflictResponse.WriteStringAsync(string.Format("Orchestration is not running or pending with {InstanceId}", instanceId));
            return conflictResponse;
        }

        var approvalDto = new ApprovalResponse
        {
            InstanceId = instanceId,
            Status = ApprovalStatus.Approved,
            ApprovalFor = ApprovalFor.Expenses
        };

        await client.RaiseEventAsync(instanceId, "ApprovalResponse", approvalDto);
        _logger.LogInformation($"ApprovalResponse event raised for instance ID: {instanceId}");
        var response = req.CreateResponse(System.Net.HttpStatusCode.OK);
        await response.WriteStringAsync(string.Format("Expense claim approved with {InstanceId}", instanceId));
        return response;

    }

    [Function("RejectExpenseClaim")]
    public async Task<HttpResponseData> RejectExpenseClaim(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "ManagerApproval/expenseReject/{instanceId}")] HttpRequestData req,
        string instanceId,
        [DurableClient] DurableTaskClient client)
    {
        // Implementation for rejecting an expense claim
        _logger.LogInformation($"Manager rejected expense claim with instance ID: {instanceId}");

        _logger.LogInformation($"Manager approved expense claim with instance ID: {instanceId}");

        // Get the status of the orchestration
        var status = await client.GetInstanceAsync(instanceId);
        if (status == null)
        {
            var notFoundResponse = req.CreateResponse(System.Net.HttpStatusCode.NotFound);
            await notFoundResponse.WriteStringAsync(string.Format("Orchestration not found with {InstanceId}", instanceId));
            return notFoundResponse;
        }
        if (status.RuntimeStatus != OrchestrationRuntimeStatus.Running &&
            status.RuntimeStatus != OrchestrationRuntimeStatus.Pending)
        {
            var conflictResponse = req.CreateResponse(System.Net.HttpStatusCode.Conflict);
            await conflictResponse.WriteStringAsync(string.Format("Orchestration is not running or pending with {InstanceId}", instanceId));
            return conflictResponse;
        }

        var approvalDto = new ApprovalResponse
        {
            InstanceId = instanceId,
            Status = ApprovalStatus.Rejected,
            ApprovalFor = ApprovalFor.Expenses
        };

        await client.RaiseEventAsync(instanceId, "ApprovalResponse", approvalDto);
        _logger.LogInformation($"ApprovalResponse event raised for instance ID: {instanceId}");
        var response = req.CreateResponse(System.Net.HttpStatusCode.OK);
        await response.WriteStringAsync(string.Format("Expense claim rejected with {InstanceId}", instanceId));
        return response;
    }
}
