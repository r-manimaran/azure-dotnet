using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace ECommerceApp;

public class OrderProcessor
{
    private readonly ILogger<OrderProcessor> _logger;

    public OrderProcessor(ILogger<OrderProcessor> logger)
    {
        _logger = logger;
    }

    [Function("ProcessOrder")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
    {
        var orderId = Guid.NewGuid().ToString();
        var random = new Random();
        int price = random.Next(10, 500);

        // Level 3 Exploration: Structured Logging
        using(_logger.BeginScope(new Dictionary<string,object>
        {
            ["OrderId"]=orderId,
            ["UserType"]=price > 400 ? "VIP":"Standard"
        }))
        {
            _logger.LogInformation("Processing order for amount: {Price}", price);

            //Simulate external dependency (Level 2: Duration/Performance)
            await Task.Delay(random.Next(100,2000));

            //Simulate random failures (Level 1 & 4 : Error tracking/Anomalies)
            if(price > 450)
            {
                _logger.LogError("Payment Gateway Timeput for Order {OrderId}", orderId);
                throw new Exception("Gateway connection lost.");
            }
        }

        
        return new OkObjectResult(new {OrderId=orderId, Status="Success"});
    }
}