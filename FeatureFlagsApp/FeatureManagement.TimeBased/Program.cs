using FeatureManagement.TimeBased;
using FeatureManagement.TimeBased.Features;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.FeatureManagement;
using Microsoft.FeatureManagement.FeatureFilters;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddHttpContextAccessor();
var azureAppConfigConnection = builder.Configuration["AppConfigConnectionString"];
builder.Configuration.AddAzureAppConfiguration(options =>
{
    options.Connect(azureAppConfigConnection)
    .ConfigureRefresh(refreshOptions =>
    {
        refreshOptions.Register("Settings:Sentinel", refreshAll: true)
                     .SetRefreshInterval(new(0, 0, 10));
    })

    .UseFeatureFlags(flagOptions =>
    {
        flagOptions.SetRefreshInterval(TimeSpan.FromSeconds(3));
        flagOptions.Select(KeyFilter.Any, LabelFilter.Null);
    });

});

builder.Services.AddFeatureManagement()
                .AddFeatureFilter<TimeWindowFilter>()
                .AddFeatureFilter<BrowserFeatureFilter>() // Custom Filter
                .AddFeatureFilter<PercentageFilter>(); 

builder.Services.AddAzureAppConfiguration();
//Mock TImeProvider
builder.Services.AddSingleton<TimeProvider>(new  MockTimeProvider(
    new DateTimeOffset(2024,12,15,0,0,0,TimeSpan.Zero)));

var app = builder.Build();

// For dynamic configuration
app.UseAzureAppConfiguration();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
