using FileProcessor;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddHeaderPropagation(opt => opt.Headers.Add("azure-correlation-id"));

builder.Services.AddHostedService<FileProcessorJob>();

builder.Host.UseSerilog((context, configuration) => configuration
            .ReadFrom.Configuration(context.Configuration));

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
