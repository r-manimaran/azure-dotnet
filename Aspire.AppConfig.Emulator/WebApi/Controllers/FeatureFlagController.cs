using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FeatureFlagController : ControllerBase
{
    private readonly IFeatureManager _featureManager;
    private readonly IConfiguration _configuration;

    public FeatureFlagController(IFeatureManager featureManager, IConfiguration configuration)
    {
        _featureManager = featureManager;
        _configuration = configuration;
    }

    [HttpGet("check/{featureName}")]
    public async Task<IActionResult> CheckFeature(string featureName)
    {
        var isEnabled = await _featureManager.IsEnabledAsync(featureName);
        return Ok(new { FeatureName = featureName, IsEnabled = isEnabled });
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAllFeatures()
    {
        var features = new[]
        {
            "NewDashboard",
            "BetaFeatures",
            "AdvancedReporting",
            "ExperimentalUI"
        };

        var results = new List<object>();
        foreach (var feature in features)
        {
            var isEnabled = await _featureManager.IsEnabledAsync(feature);
            results.Add(new { FeatureName = feature, IsEnabled = isEnabled });
        }

        return Ok(results);
    }

    [HttpGet("percentage/{featureName}")]
    public IActionResult GetPercentageFeature(string featureName)
    {
        var percentage = _configuration.GetValue<int>($"FeatureManagement:{featureName}:EnabledFor:0:Parameters:Value", 0);
        return Ok(new { FeatureName = featureName, Percentage = percentage });
    }
}