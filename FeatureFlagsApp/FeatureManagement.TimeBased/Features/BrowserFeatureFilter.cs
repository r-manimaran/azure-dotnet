using Microsoft.FeatureManagement;

namespace FeatureManagement.TimeBased.Features;

/// <summary>
/// Custom Feature 
/// </summary>
[FilterAlias("BrowserFilter")]
public class BrowserFeatureFilter : IFeatureFilter
{
    private readonly IHttpContextAccessor _contextAccessor;
    public BrowserFeatureFilter(IHttpContextAccessor contextAccessor)
    {
        _contextAccessor = contextAccessor;
    }
    public Task<bool> EvaluateAsync(FeatureFilterEvaluationContext context)
    {
        var userAgent = _contextAccessor.HttpContext.Request.Headers["User-Agent"].ToString();

        var settings = context.Parameters.Get<BrowserFilterSettings>();

        return Task.FromResult(settings.AllowedBrowsers.Any(userAgent.Contains));
    }
}
