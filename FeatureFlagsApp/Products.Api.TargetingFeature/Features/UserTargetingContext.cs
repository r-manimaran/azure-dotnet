using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.FeatureManagement.FeatureFilters;

namespace Products.Api.TargetingFeature.Features;

internal sealed class UserTargetingContext : ITargetingContextAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private const string CacheKey = "UserTargetContext.TargetingContext";

    public UserTargetingContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    
    public ValueTask<TargetingContext> GetContextAsync()
    {
       var httpContext = _httpContextAccessor.HttpContext!;
        if(httpContext.Items.TryGetValue(CacheKey, out object? value))
        {
            return new ValueTask<TargetingContext>((TargetingContext)value!);
        }

        var targetingContext = new TargetingContext
        {
            UserId = GetUserId(httpContext),
            Groups = GetUserGroups(httpContext)
        };
        httpContext.Items.Add(CacheKey, targetingContext);

        return new ValueTask<TargetingContext>(targetingContext);
    }


    private static string GetUserId(HttpContext? httpContext)
    {
        // use authentication Claims to get the username. 
        return httpContext?.Request.Headers["x-user-id"].FirstOrDefault() ?? string.Empty;
    }

    private static string[] GetUserGroups(HttpContext? httpContext)
    {
        // Use Claims or Database to get the user associated groups.
        var userGroups = httpContext?.Request
                         .Headers["x-user-groups"]
                         .FirstOrDefault()?
                         .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                         ?? [];
        return userGroups;
    }
}
