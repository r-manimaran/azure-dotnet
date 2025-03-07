using static System.Reflection.Metadata.BlobBuilder;
using Microsoft.Extensions.Configuration;

var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

var blobs = builder.AddConnectionString("blobs");

var apiService = builder.AddProject<Projects.AspireApp_AzureBlob_ApiService>("apiservice")
                        .WithReference(blobs);

builder.AddProject<Projects.AspireApp_AzureBlob_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(cache)
    .WaitFor(cache)
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
