using Azure.Core;
using Azure.Identity;
using System.Diagnostics;

namespace WebApp;

public static class Extension
{
    public static void AddAzureAppConfiguration(this WebApplicationBuilder builder, string connectionStringName="appConfig")
    {
       string? connectionString = builder.Configuration.GetConnectionString(connectionStringName);
       if (string.IsNullOrEmpty(connectionString))
       {
            Debug.WriteLine($"Connection string '{connectionStringName}' is not configured.");
            return;
       }
       Debug.WriteLine($"Using Azure App Configuration with connection string '{connectionStringName}'.");

        string? clientId = builder.Configuration["AZURE_CLIENT_ID"];

        TokenCredential credential = string.IsNullOrEmpty(clientId)
            ? new DefaultAzureCredential(includeInteractiveCredentials:builder.Environment.IsDevelopment())
            : new ManagedIdentityCredential(clientId);
        Debug.WriteLine($"Using Azure App Configuration with client ID '{clientId}'.");
        builder.Configuration.AddAzureAppConfiguration(options =>
        {
            options.Connect(new Uri(connectionString), credential);
            options.ConfigureKeyVault(kv=>kv.SetCredential(credential));
        });
        Debug.WriteLine("Azure App Configuration added to the configuration builder.");
    }
}
