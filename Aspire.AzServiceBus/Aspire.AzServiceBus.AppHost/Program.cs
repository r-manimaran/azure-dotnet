using Microsoft.Extensions.DependencyInjection;

var builder = DistributedApplication.CreateBuilder(args);

var serviceBus = builder.AddAzureServiceBus("sbemulator").RunAsEmulator();

var serviceBusQueue = serviceBus.AddServiceBusQueue("insurancepolicies");

// For ServiceBusTopic
//var serviceBusTopic = serviceBus.AddServiceBusTopic("insurance");


var client = builder.AddProject<Projects.AzServiceBusClient>("azservicebusclient")
                    .WithReference(serviceBusQueue)
                    .WaitFor(serviceBusQueue);

builder.AddProject<Projects.BlazorWebApp>("blazorwebapp")
       .WithReference(serviceBusQueue)
       .WaitFor(client);

builder.Build().Run();
