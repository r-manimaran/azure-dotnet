
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using AzureUtilities;
using Carter;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthorization();

builder.Services.AddOpenApi();

builder.Services.AddLogging();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddCarter();

//Get the Service Prefix from the appSettings
var servicePrefix = builder.Configuration["ServicePrefix"] ??
    throw new InvalidOperationException("ServicePrefix is not configured in appSettings.json or environment variables.");

// Fetch the Valid Prefixes from KeyVault
string validPrefixes;

//Configure Azure Key Valut with ServicePrefixKeyVaultSecretManager
var keyVaultUri = new Uri(builder.Configuration["KeyVaultUri"] ??
                Environment.GetEnvironmentVariable("KeyVaultUrl") ??
                throw new InvalidOperationException($"KeyVaultUri is missing"));

// validate Secrets
var client = new SecretClient(keyVaultUri, new DefaultAzureCredential());
var prefixSecret = client.GetSecret("ServicePrefixes").Value;
validPrefixes = prefixSecret.Value;

string prefixDelimeter = "--";
var secretManager = new ServicePrefixKeyVaultSecretManager(servicePrefix,
    prefixDelimeter,
    validPrefixes,
    LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<ServicePrefixKeyVaultSecretManager>());

builder.Configuration.AddAzureKeyVault(keyVaultUri,
    new DefaultAzureCredential(),
    secretManager);

var secrets = new List<KeyVaultSecret>();

foreach (var secretProperties in client.GetPropertiesOfSecrets())
{
    var secret = client.GetSecret(secretProperties.Name).Value;
    secrets.Add(secret);
}

secretManager.ValidateSecrets(secrets, "Connection","ApiEndpoint");

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseSwaggerUI(c => c.SwaggerEndpoint("/openapi/v1.json", "OpenAPI V1"));

app.MapCarter();

app.UseHttpsRedirection();

app.UseAuthorization();

app.Run();
