using FunctionApp.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

namespace FunctionApp.Functions;

public static class DatabaseActivities
{

    [Function(nameof(SaveRequest))]
    public static async Task<string> SaveRequest([ActivityTrigger] object input, FunctionContext context)
    {
        var serviceProvider = context.InstanceServices;
        var dbContext = serviceProvider.GetRequiredService<AppDbContext>();

        dynamic requestData = input;
        var request = requestData.Request;
        var type = (RequestType)requestData.Type;

        var entity = new RequestEntity
        {
            Id = request.RequestId,
            InstanceId = requestData.InstanceId,
            EmployeeId = requestData.EmployeeId,
            EmployeeName = requestData.EmployeeName,
            Type = type,
            RequestData = JsonSerializer.Serialize(request),
            Status = RequestStatus.Submitted,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        dbContext.Requests.Add(entity);
        await dbContext.SaveChangesAsync();
        return entity.Id;
    }


    [Function(nameof(UpdateRequestStatus))]
    public static async Task UpdateRequestStatus([ActivityTrigger] object input, FunctionContext context)
    {
        var serviceProdiver = context.InstanceServices;
        var dbContext = serviceProdiver.GetRequiredService<AppDbContext>();

        dynamic statusUpdate = input;
        string requestId = statusUpdate.RequestId;
        RequestStatus status = statusUpdate.Status;
        string comments = statusUpdate.Comments ?? "";

        var entity = await dbContext.Requests.FirstOrDefaultAsync(r=>r.Id == requestId);
        if (entity != null)
        {
            entity.Status = status;
            entity.Comments = comments;
            entity.UpdatedAt = DateTime.UtcNow;
            await dbContext.SaveChangesAsync();
        }           

    }
}
