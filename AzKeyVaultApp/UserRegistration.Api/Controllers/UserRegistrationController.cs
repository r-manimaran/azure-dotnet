using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UserRegistration.Api.Dtos;
using UserRegistration.Api.Services;

namespace UserRegistration.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserRegistrationController : ControllerBase
    {
        private readonly IUserRegistrationService _userRegistrationService;
        private readonly ILogger<UserRegistrationController> _logger;

        public UserRegistrationController(IUserRegistrationService userRegistrationService,
                                          ILogger<UserRegistrationController> logger)
        {
            _userRegistrationService = userRegistrationService;
            _logger = logger;
        }
        [HttpPost]
        public async Task<IActionResult> CreateRegistration([FromBody] Request request)
        {
            var response = await _userRegistrationService.RegisterUser(request);
            return Ok(response);
        }

        [HttpGet("VerifyEmail/{token}")]
        public async Task<IActionResult> VerifyEmail(string token)
        {
            return Ok();
        }
    }
}
