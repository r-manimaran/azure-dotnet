using AzureKeyVaultEmulator.Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var keyVaultResourceName = "keyvault";
var keyVault = builder
    .AddAzureKeyVault(keyVaultResourceName)
    .RunAsEmulator();

builder.AddProject<Projects.WebApi>("webapi")
    .WithReference(keyVault);

builder.Build().Run();
