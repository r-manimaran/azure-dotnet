using UserRegistration.Api.Controllers;
using UserRegistration.Api.Models;

namespace UserRegistration.Api.Services;

public class EmailVerificationLinkService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly LinkGenerator _linkGenerator;

    public EmailVerificationLinkService(IHttpContextAccessor httpContextAccessor, LinkGenerator linkGenerator)
    {
        _httpContextAccessor = httpContextAccessor;
        _linkGenerator = linkGenerator;
    }

    public string Create(EmailVerificationToken emailVerificationToken)
    {
       
          var httpContext = _httpContextAccessor.HttpContext 
            ?? throw new InvalidOperationException("HttpContext is not available");


         var scheme = httpContext.Request.Scheme;
        var host = httpContext.Request.Host.Value;

        // Corrected to match the controller route name
        string? verificationLink = _linkGenerator.GetUriByAction(
            httpContext: httpContext,
            action: nameof(UserRegistrationController.VerifyEmail),
            controller: "UserRegistration",
            values: new { token = emailVerificationToken.Id },
            scheme: scheme);

        return verificationLink ?? throw new Exception("Could not create email verification link");
    }
}
