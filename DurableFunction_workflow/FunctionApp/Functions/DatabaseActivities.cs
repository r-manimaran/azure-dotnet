using FunctionApp.DTOs;
using FunctionApp.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

namespace FunctionApp.Functions;

public static class DatabaseActivities
{

    [Function(nameof(SaveRequest))]
    public static async Task<string> SaveRequest([ActivityTrigger] SaveRequestInput input, FunctionContext context)
    {
       var serviceProvider = context.InstanceServices;
 

        var dbContext = serviceProvider.GetRequiredService<AppDbContext>();

        var requestJson = JsonSerializer.Serialize(input.Request);
        var requestElement = JsonSerializer.Deserialize<JsonElement>(requestJson);

        var entity = new RequestEntity
        {
            Id = requestElement.GetProperty("RequestId").GetString()!,
            InstanceId = input.InstanceId,
            EmployeeId = requestElement.GetProperty("EmployeeId").GetString()!,
            EmployeeName = requestElement.GetProperty("EmployeeName").GetString()!,
            Type = input.Type,
            RequestData = requestJson,
            Status = RequestStatus.Submitted,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        dbContext.Requests.Add(entity);
        await dbContext.SaveChangesAsync();
        return entity.Id;
    }


    [Function(nameof(UpdateRequestStatus))]
    public static async Task UpdateRequestStatus([ActivityTrigger] UpdateStatusInput input, FunctionContext context)
    {
        var serviceProdiver = context.InstanceServices;
        var dbContext = serviceProdiver.GetRequiredService<AppDbContext>();


        var entity = await dbContext.Requests.FirstOrDefaultAsync(r => r.Id == input.RequestId);
        if (entity != null)
        {
            entity.Status = input.Status;
            entity.Comments = input.Comments ?? "";
            entity.UpdatedAt = DateTime.UtcNow;
            await dbContext.SaveChangesAsync();
        }
    }
}
