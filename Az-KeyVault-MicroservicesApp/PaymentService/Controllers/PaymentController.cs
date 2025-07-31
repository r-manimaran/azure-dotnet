using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PaymentService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public PaymentController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        [HttpGet("apiKey")]
        public IActionResult GetApiKey()
        {
            string apiKey = _configuration["ApiKey"] ?? "ApiKey not found";
            return Ok
            (
               new {
                    ApiKey = apiKey
                }
            );
        }
    }
}
