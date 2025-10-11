using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConfigController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<ConfigController> _logger;

    public ConfigController(IConfiguration configuration, ILogger<ConfigController> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    [HttpGet("database")]
    public IActionResult GetDatabaseConfig()
    {
        var connectionString = _configuration.GetConnectionString("DefaultConnection");
        var timeout = _configuration.GetValue<int>("Database:CommandTimeout", 30);
        var retryCount = _configuration.GetValue<int>("Database:RetryCount", 3);

        return Ok(new
        {
            ConnectionString = connectionString?.Substring(0, Math.Min(20, connectionString.Length)) + "...",
            CommandTimeout = timeout,
            RetryCount = retryCount
        });
    }

    [HttpGet("features")]
    public IActionResult GetFeatureFlags()
    {
        var enableNewUI = _configuration.GetValue<bool>("Features:EnableNewUI", false);
        var enableBetaFeatures = _configuration.GetValue<bool>("Features:EnableBetaFeatures", false);
        var maxFileSize = _configuration.GetValue<long>("Features:MaxFileUploadSize", 10485760);

        return Ok(new
        {
            EnableNewUI = enableNewUI,
            EnableBetaFeatures = enableBetaFeatures,
            MaxFileUploadSizeMB = maxFileSize / 1024 / 1024
        });
    }

    [HttpGet("api-settings")]
    public IActionResult GetApiSettings()
    {
        var rateLimitPerMinute = _configuration.GetValue<int>("Api:RateLimitPerMinute", 100);
        var enableCaching = _configuration.GetValue<bool>("Api:EnableCaching", true);
        var cacheExpirationMinutes = _configuration.GetValue<int>("Api:CacheExpirationMinutes", 15);

        return Ok(new
        {
            RateLimitPerMinute = rateLimitPerMinute,
            EnableCaching = enableCaching,
            CacheExpirationMinutes = cacheExpirationMinutes
        });
    }

    [HttpGet("environment")]
    public IActionResult GetEnvironmentConfig()
    {
        var environment = _configuration.GetValue<string>("Environment", "Development");
        var logLevel = _configuration.GetValue<string>("Logging:LogLevel:Default", "Information");
        var enableDetailedErrors = _configuration.GetValue<bool>("DetailedErrors", false);

        return Ok(new
        {
            Environment = environment,
            LogLevel = logLevel,
            EnableDetailedErrors = enableDetailedErrors
        });
    }
}