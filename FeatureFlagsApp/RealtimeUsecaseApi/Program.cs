using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.FeatureManagement;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddFeatureManagement();
builder.Services.AddAzureAppConfiguration();

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
               flagOptions.SetRefreshInterval(TimeSpan.FromSeconds(5));
               //Specify what Feature Flag needs to be in the configuration.
               flagOptions.Select(KeyFilter.Any,LabelFilter.Null);
           });
})
                ;


//If you are having the configuration settings other than FeatureManagement
//in the appSettings.json, we can define the section in the delegate

// builder.Services.AddFeatureManagement(builder.Configuration.GetSection("SectionName"));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

//For dynamic configuration
app.UseAzureAppConfiguration();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
