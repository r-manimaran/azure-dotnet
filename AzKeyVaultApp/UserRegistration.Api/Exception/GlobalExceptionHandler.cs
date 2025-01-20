using Microsoft.AspNetCore.Mvc;
using Polly;
using System.Net;
using System.Text.Json;

namespace UserRegistration.Api;

public class GlobalExceptionHandler
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandler> _logger;
    public GlobalExceptionHandler(RequestDelegate next, ILogger<GlobalExceptionHandler> logger)
    {
        _next = next;
        _logger = logger;
    }
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch(DllNotFoundException ex)
        {
            _logger.LogError(ex, ex.Message);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            await HandleExceptionAsync(context, ex);
        }
        catch(ArgumentException ex)
        {
            _logger.LogError(ex, ex.Message);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            await HandleExceptionAsync(context, ex);
        } 
        catch(UnauthorizedAccessException ex)
        {
            _logger.LogError(ex, ex.Message);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            await HandleExceptionAsync(context, ex);
        }       
        catch(Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            await HandleExceptionAsync(context, ex);
        }
    }

    public static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        var response = new 
        {
            error = new 
            {
                message = exception.Message,
                statusCode = context.Response.StatusCode,
                exceptionType = exception.GetType().Name,
                exceptionSource = exception.TargetSite?.DeclaringType?.FullName,
                exceptionMethod = exception.TargetSite?.Name,                
                stackTrace = exception.StackTrace            
            },
            timestamp = DateTime.UtcNow
        };
        
        await context.Response.WriteAsync(JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            WriteIndented = true
        }));
    }
}
