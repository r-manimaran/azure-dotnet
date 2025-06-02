using Aspire.Hosting.Azure;

var builder = DistributedApplication.CreateBuilder(args);

IResourceBuilder<AzureAppConfigurationResource>? appConfig = null;

if (builder.ExecutionContext.IsPublishMode)
{
    // In publish mode, we can use the Azure App Configuration resource to load configuration.
    appConfig = builder.AddAzureAppConfiguration("appconfig");
    builder.AddAzureKeyVault("keyvault");
}

    var app = builder.AddProject<Projects.WebApp>("webapp")
                  .WithExternalHttpEndpoints();

    if(appConfig != null)
    {
        // Add the Azure App Configuration resource to the web application.
        app.WithReference(appConfig);
    }
builder.Build().Run();
