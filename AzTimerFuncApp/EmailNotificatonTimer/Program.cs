using AzureStorageUtility;
using EmailNotificatonTimer;
using EmailNotificatonTimer.Services;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Services.AddLogging(logging=>
    {
        logging.AddSerilogLogger(); // Custom extension method to add Serilog logger
    });

builder.Services.AddTransient<AzureBlobStorageUtility>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var logger = sp.GetRequiredService<ILogger<AzureBlobStorageUtility>>();

    // Get values from configuration or environment variables
    var storageAccountUrlOrConnectionString = Environment.GetEnvironmentVariable("BlobStorageUri"); ;
    var authMode = Enum.Parse<BlobStorageAuthMode>(Environment.GetEnvironmentVariable("AuthMode"));
    var keyVaultUri = Environment.GetEnvironmentVariable("KeyVaultUri"); ;
    var secretName = Environment.GetEnvironmentVariable("KeyVaultSecretName");
    var clientId = Environment.GetEnvironmentVariable("ManagedIdentityClientId");

    return new AzureBlobStorageUtility(
        storageAccountUrlOrConnectionString, 
        authMode, 
        keyVaultUri, 
        secretName, 
        clientId, 
        logger);
});

builder.Services.AddSingleton<EmployeeService>();

// Application Insights isn't enabled by default. See https://aka.ms/AAt8mw4.
// builder.Services
//     .AddApplicationInsightsTelemetryWorkerService()
//     .ConfigureFunctionsApplicationInsights();

builder.Build().Run();
