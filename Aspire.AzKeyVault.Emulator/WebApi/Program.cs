using Azure.Security.KeyVault.Keys;
using Azure.Security.KeyVault.Secrets;
using AzureKeyVaultEmulator.Aspire.Client;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddOpenApi();

var vaultUri = builder.Configuration.GetConnectionString("keyvault") ?? string.Empty;

if (builder.Environment.IsDevelopment())
{
    //builder.Services.AddAzureKeyVaultEmulator(vaultUri);

    // Or configure which clients you need to use
    builder.Services.AddAzureKeyVaultEmulator(vaultUri, secrets: true, keys: true, certificates: false);
}
else
{
   // builder.Services.AddAzureClients(client)
}
var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/secret/{name}", async (string name, SecretClient secretClient) =>
{
    var secret = await secretClient.GetSecretAsync(name);
    
    return secret.Value;
});

app.MapGet("/key/{name}", async (string name, KeyClient keyClient) =>
{
    var key = await keyClient.GetKeyAsync(name);
    return key.Value;
});


app.Run();


