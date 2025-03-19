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
        TimeZone = "UTC"
    };
});

app.Run();

