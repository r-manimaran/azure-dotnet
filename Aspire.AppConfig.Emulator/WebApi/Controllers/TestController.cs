using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public TestController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpGet("connection-test")]
    public IActionResult TestConnection()
    {
        var connectionString = _configuration.GetConnectionString("DefaultConnection");
        
        return Ok(new
        {
            Found = !string.IsNullOrEmpty(connectionString),
            Value = connectionString?.Substring(0, Math.Min(30, connectionString?.Length ?? 0)) + "...",
            AllConnectionStrings = _configuration.GetSection("ConnectionStrings").GetChildren()
                .ToDictionary(x => x.Key, x => x.Value?.Substring(0, Math.Min(20, x.Value?.Length ?? 0)) + "...")
        });
    }
}