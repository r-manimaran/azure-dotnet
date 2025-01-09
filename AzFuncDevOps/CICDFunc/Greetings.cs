using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace CICDFunc
{
    public class Greetings
    {
        private readonly ILogger<Greetings> _logger;

        public Greetings(ILogger<Greetings> logger)
        {
            _logger = logger;
        }

        [Function("Greet")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            return new OkObjectResult("Welcome to Azure Functions - CI2!");
        }
    }
}
