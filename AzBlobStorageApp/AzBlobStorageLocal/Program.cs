using AzBlobStorageLocal.Endpoints;
using AzBlobStorageLocal.Extensions;
using AzBlobStorageLocal.Services;
using Azure.Storage.Blobs;
using Scalar.AspNetCore;
using Serilog;
using Serilog.Enrichers;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

// Alternate Approach to Register the Services
//builder.Services.AddEndpoints(Assembly.GetExecutingAssembly()); 

builder.Services.AddSingleton<IBlobService, BlobService>();
builder.Services.AddSingleton(_ => 
        new BlobServiceClient(
            builder.Configuration.GetConnectionString("BlobStorage")
                            ));
// Add Serilog
builder.Services.AddHeaderPropagation(opt => opt.Headers.Add("azure-correlation-id"));
builder.Host.UseSerilog((context, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration));
    
var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.MapScalarApiReference();

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
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

//Alternative way to register the Endpoints
//app.MapEndpoints();

app.MapBlobStorageEndpoint();
app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
