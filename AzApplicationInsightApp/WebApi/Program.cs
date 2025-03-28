using Microsoft.ApplicationInsights.Extensibility;
using Serilog;
using Serilog.Templates;
using WebApi;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
// Get the connection string from configuration
var appInsightsConnectionString = builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"];

builder.Services.AddApplicationInsightsTelemetry();

//Log.Logger = new LoggerConfiguration()
//                .WriteTo.Console()
//                .WriteTo.Debug()
//                .MinimumLevel.Information()
//                .Enrich.FromLogContext()
//                .WriteTo.File("log.txt")
//                .CreateBootstrapLogger();

builder.Services.AddSingleton<ITelemetryInitializer, CustomTelemetryInitializer>();

builder.Host.UseSerilog((context, services, loggerConfiguration) => loggerConfiguration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .WriteTo.Console(new ExpressionTemplate(
                 "[{@t:HH:mm:ss} {@l:u3}{#if @tr is not null} ({substring(@tr,0,4)}:{substring(@sp,0,4)}){#end}] {@m}\n{@x}"))
            .WriteTo.ApplicationInsights(
                appInsightsConnectionString, TelemetryConverter.Traces));

builder.Services.AddControllers();

builder.Services.AddOpenApi();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
