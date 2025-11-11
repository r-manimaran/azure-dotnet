using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace FunctionApp;

public class GetSecret
{
    private readonly ILogger<GetSecret> _logger;

    public GetSecret(ILogger<GetSecret> logger)
    {
        _logger = logger;
    }

    [Function("GetSecret")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");
        string connectionString = Environment.GetEnvironmentVariable("DbConnectionString") ?? "";
        _logger.LogInformation(connectionString);
        return new OkObjectResult("Welcome to Azure Functions!");
    }
}