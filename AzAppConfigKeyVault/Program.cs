using AzAppConfigKeyVault;
using Azure.Identity;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Host.ConfigureAppConfiguration(opt =>
{
    var credential = new DefaultAzureCredential();
    var connectionString = builder.Configuration.GetConnectionString("AzureAppConfiguration");
    
    if (string.IsNullOrEmpty(connectionString))
    {
        throw new ArgumentNullException("AzureAppConfiguration connection string is not configured");
    }

    opt.AddAzureAppConfiguration(options =>
    {
        options.Connect(connectionString)
              .ConfigureKeyVault(kv =>
              {
                  kv.SetCredential(credential);
                  kv.SetSecretRefreshInterval(TimeSpan.FromSeconds(60));
              })
              .ConfigureRefresh(refresh =>
              {
                  // Use constants for refresh intervals to make maintenance easier
                  const int refreshIntervalSeconds = 15;
                  
                  refresh.Register($"{AppConfOptions.AppConfigOptionKey}:{nameof(AppConfOptions.FirstConfig)}")
                         .SetRefreshInterval(TimeSpan.FromSeconds(refreshIntervalSeconds));
                  refresh.Register($"{AppConfOptions.AppConfigOptionKey}:{nameof(AppConfOptions.SecondConfig)}")
                         .SetRefreshInterval(TimeSpan.FromSeconds(refreshIntervalSeconds));

                  // For Weather Controller
                  refresh.Register($"{WeatherConfiguration.prefixName}:{nameof(WeatherConfiguration.Count)}")
                         .SetRefreshInterval(TimeSpan.FromSeconds(refreshIntervalSeconds));
              })
              .UseFeatureFlags(); // Add feature flags support if needed
    });
});


builder.Services.AddAzureAppConfiguration();
builder.Services.Configure<AppConfOptions>(builder.Configuration.GetSection(AppConfOptions.AppConfigOptionKey));
builder.Services.Configure<WeatherConfiguration>(builder.Configuration.GetSection(WeatherConfiguration.prefixName));

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseAzureAppConfiguration();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
