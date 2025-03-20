var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var app = builder.Build();

app.MapOpenApi();

app.UseSwaggerUI(opt =>
    opt.SwaggerEndpoint("/openapi/v1.json", "OpenAPI v1"));

app.UseHttpsRedirection();

app.MapGet("/time", () =>
{
    return new
    {
        CurrentTime = DateTime.UtcNow,
        TimeZone = "UTC",
        UnixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
        IsDaylightSavingTime = TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.UtcNow),
        Formats = new {
            RFC1123 = DateTime.UtcNow.ToString("r"),
            ISO8601 = DateTime.UtcNow.ToString("o"),
            UnixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()
    
        }
    };
});

app.MapGet("/health",()=> {
    return Results.Ok(new {
        Status="Healthy",
        Version ="1.0.0",
        Timestamp = DateTime.UtcNow.ToString("o")
    });
});

app.Run();

