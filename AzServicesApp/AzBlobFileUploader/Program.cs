using AzBlobFileUploader.Blob;
using AzBlobFileUploader.Container;
using AzBlobFileUploader.Endpoints;
using Azure.Storage.Blobs;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
// Add Serilog
builder.Services.AddHeaderPropagation(opt => opt.Headers.Add("azure-correlation-id"));

builder.Host.UseSerilog((context, configuration) => configuration
            .ReadFrom.Configuration(context.Configuration));

builder.Services.AddSingleton(_ =>
        new BlobServiceClient(
            builder.Configuration.GetConnectionString("BlobStorage")
                            ));

builder.Services.AddScoped<IBlobService, BlobService>();

builder.Services.AddScoped<IContainerService, ContainerService>();

var app = builder.Build();

app.MapOpenApi();

app.UseSwaggerUI(opt =>
    opt.SwaggerEndpoint("/openapi/v1.json", "OpenAPI v1"));

app.MapGet("/",() => "Hello world");

app.MapBlobEndpoints();

app.Run();
