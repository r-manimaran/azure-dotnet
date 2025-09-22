using Microsoft.AspNetCore.Builder;
using WeatherApi.Endpoints;
using WeatherApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddScoped<GitHubService>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
     policy.AllowAnyOrigin()
     .AllowAnyHeader()
     .AllowAnyMethod()
     .SetIsOriginAllowed(_ => true));
});

builder.Services.AddHttpClient("GithubClient", opt =>
{
    opt.BaseAddress = new Uri("https://api.github.com/");

    opt.DefaultRequestHeaders.Add("User-Agent", "GitHub-Integration-App");
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options => {
        options.SwaggerEndpoint(
        "/openapi/v1.json", "OpenAPI v1");
    });
}

app.UseCors();

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{

    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ));
        
    return forecast;

}).WithName("GetWeatherForecast");

app.MapGitHubEndpoints();

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
