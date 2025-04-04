using AzFileProcessing.Common.Services;
using Azure.Storage.Queues;
using FileIngestionProcessor;
using MessagingServiceProvider;
using MessagingServiceProvider.AzureServiceBus;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddHeaderPropagation(opt => opt.Headers.Add("azure-correlation-id"));

builder.Services.AddOptions<AzureSetting>().BindConfiguration(AzureSetting.SectionName);

builder.Services.AddOptions<AzureServiceBusSettings>().BindConfiguration(AzureServiceBusSettings.SectionName);

builder.Services.AddScoped<IEventService, EventService>();

builder.Services.AddSingleton<IMessageService, AzServiceBusService>();

builder.Host.UseSerilog((context, configuration) => configuration
            .ReadFrom.Configuration(context.Configuration));

builder.Services.AddSingleton<QueueClient>(opt => new QueueClient(builder.Configuration.GetConnectionString("StorageQueue"), builder.Configuration["AzureSetting:StorageQueueName"]));

builder.Services.AddHostedService<IngestionBackgroundService>();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
