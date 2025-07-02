using AzServiceBusClient.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

builder.AddAzureServiceBusClient("insurancepolicies");

CancellationTokenSource cTokenSource = new();

CancellationToken cToken = cTokenSource.Token;

builder.Services.AddKeyedSingleton("AppShutdown", cTokenSource);

builder.Services.AddHostedService<InsurancePolicyBusService>();

using IHost host = builder.Build();

await host.RunAsync(cToken).ConfigureAwait(false);
