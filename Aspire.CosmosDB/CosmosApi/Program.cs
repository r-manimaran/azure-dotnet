using Azure.Core;
using CosmosApi;
using CosmosApi.Endpoints;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddOpenApi();

builder.AddAzureCosmosClient("cosmos",
    settings =>
    {
        settings.DisableTracing = false;
    },
    clientOptions =>
    {
        clientOptions.ApplicationName = "cosmos-aspire";
        clientOptions.SerializerOptions = new()
        {
            PropertyNamingPolicy = Microsoft.Azure.Cosmos.CosmosPropertyNamingPolicy.Default
        };
        clientOptions.CosmosClientTelemetryOptions = new()
        {
            DisableDistributedTracing = false
        };
        clientOptions.ConnectionMode = ConnectionMode.Gateway;
    });

builder.Services.AddSingleton<DatabaseBootstrapper>();
builder.Services.AddHealthChecks()
       .Add(new("cosmos",sp => sp.GetRequiredService<DatabaseBootstrapper>(),null,null));

builder.Services.AddHostedService(sp=>sp.GetRequiredService<DatabaseBootstrapper>());


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
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

app.MapCosmosEndpoints();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
