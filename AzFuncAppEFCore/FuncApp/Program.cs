using FuncApp;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

var logger = builder.Services.BuildServiceProvider()
    .GetRequiredService<Microsoft.Extensions.Logging.ILogger<Program>>();
logger.LogInformation("Starting up");
var connectionString = Environment.GetEnvironmentVariable("Sqldb");
if (!string.IsNullOrEmpty(connectionString))
{
    logger.LogInformation($"Using provided SQL connection string.:{connectionString}");
    builder.Services.AddDbContext<AppDbContext>(options => 
        options.UseSqlServer(connectionString));
}
else
{
    // Fallback for local development
    logger.LogWarning("Environment variable 'Sqldb' not found. Using localdb for development.");
    builder.Services.AddDbContext<AppDbContext>(options => 
        options.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=TempDb;Trusted_Connection=true;"));
}

//using( var scope = builder.Services.BuildServiceProvider().CreateScope())
//{
//    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
//    dbContext.Database.EnsureCreated();
//}

builder.Build().Run();
