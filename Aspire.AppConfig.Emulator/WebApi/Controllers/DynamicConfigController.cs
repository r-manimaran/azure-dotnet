using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DynamicConfigController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly IOptionsSnapshot<AppSettings> _appSettings;

    public DynamicConfigController(IConfiguration configuration, IOptionsSnapshot<AppSettings> appSettings)
    {
        _configuration = configuration;
        _appSettings = appSettings;
    }

    [HttpGet("refresh")]
    public IActionResult RefreshConfiguration()
    {
        if (_configuration is IConfigurationRoot configRoot)
        {
            configRoot.Reload();
            return Ok(new { Message = "Configuration refreshed", Timestamp = DateTime.UtcNow });
        }
        return BadRequest("Configuration refresh not supported");
    }

    [HttpGet("app-settings")]
    public IActionResult GetAppSettings()
    {
        return Ok(_appSettings.Value);
    }

    [HttpGet("connection-strings")]
    public IActionResult GetConnectionStrings()
    {
        var connectionStrings = new Dictionary<string, string>();
        var section = _configuration.GetSection("ConnectionStrings");
        
        foreach (var child in section.GetChildren())
        {
            var value = child.Value;
            connectionStrings[child.Key] = value?.Length > 20 ? value.Substring(0, 20) + "..." : value ?? "";
        }

        return Ok(connectionStrings);
    }

    [HttpGet("section/{sectionName}")]
    public IActionResult GetConfigSection(string sectionName)
    {
        var section = _configuration.GetSection(sectionName);
        if (!section.Exists())
        {
            return NotFound($"Configuration section '{sectionName}' not found");
        }

        var result = new Dictionary<string, string>();
        foreach (var child in section.GetChildren())
        {
            result[child.Key] = child.Value ?? "";
        }

        return Ok(result);
    }
}

public class AppSettings
{
    public string ApplicationName { get; set; } = "";
    public string Version { get; set; } = "";
    public int MaxConcurrentUsers { get; set; }
    public bool EnableMetrics { get; set; }
}