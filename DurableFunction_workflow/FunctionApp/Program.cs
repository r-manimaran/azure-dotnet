using Azure;
using FunctionApp;
using FunctionApp.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SendGrid;
using SendGrid.Extensions.DependencyInjection;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(Environment.GetEnvironmentVariable("SqlConnectionString")));

builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

builder.Services.AddScoped<ITemplateService, TemplateService>();
// Register Email Service based on environment
var isDevelopment = Environment.GetEnvironmentVariable("AZURE_FUNCTIONS_ENVIRONMENT") == "Development";
if (isDevelopment)
{
    builder.Services.AddScoped<IEmailService, LocalEmailService>();
}
else
{
    builder.Services.AddSendGrid(Opt =>
    {
        Opt.ApiKey = Environment.GetEnvironmentVariable("SendGridApiKey");
    });
    builder.Services.AddScoped<IEmailService, EmailService>();
}
    
var app = builder.Build();

// Apply migrations on startup
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    context.Database.Migrate();
}

app.Run();