using Asp.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Products.Api.TargetingFeature.Data;
using Products.Api.TargetingFeature.Extensions;
using Scalar.AspNetCore;
using Microsoft.FeatureManagement;
using Products.Api.TargetingFeature.Features;
using Azure.Identity;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

#region If using Endpoint and DefaultAzureCredentials using Azure.Identity
/*
// Retrieve the endpoint
string endpoint = builder.Configuration.GetValue<string>("Endpoints:AppConfiguration")
    ?? throw new InvalidOperationException("The setting `Endpoints:AppConfiguration` was not found.");

// Connect to Azure App Configuration and load all feature flags with no label
builder.Configuration.AddAzureAppConfiguration(options =>
{
    options.Connect(new Uri(endpoint), new DefaultAzureCredential())
           .UseFeatureFlags(flagOptions =>
           {
               flagOptions.SetRefreshInterval(new(0, 0, 5));
           });
});
*/
#endregion

//For Using Connection String
// Retrieve the connection string
string azAppConnectionString = builder.Configuration.GetConnectionString("AppConfiguration")
    ?? throw new InvalidOperationException("The connection string 'AppConfiguration' was not found.");

// Connect to Azure App Configuration and load all feature flags with no label
builder.Configuration.AddAzureAppConfiguration(options =>
{
    options.Connect(azAppConnectionString)
           .UseFeatureFlags(flagOptions =>
           {
               flagOptions.SetRefreshInterval(new(0, 0, 5));
           });
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddSwaggerGen(options =>
{
    // Add a swagger document for each discovered API version
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Products API V1",
        Version = "v1",
        Description = "A simple example Products API",
    });

    options.SwaggerDoc("v2", new OpenApiInfo
    {
        Title = "Products API V2",
        Version = "v2",
        Description = "A simple example Products API with enhanced features",
    });
});

builder.Services.AddOpenApi();

builder.Services.AddDbContext<ProductDbContext>(options =>
     options.UseInMemoryDatabase("ProductsDb"));

builder.Services.AddScoped<ProductSeeder>();

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader(),
        new HeaderApiVersionReader("x-api-version"),
        new MediaTypeApiVersionReader("x-api-version"));

})
.AddMvc()
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddAzureAppConfiguration();

builder.Services.AddFeatureManagement()
                .WithTargeting<UserTargetingContext>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Products API V1");
        options.SwaggerEndpoint("/swagger/v2/swagger.json", "Products API V2");
    });

    app.MapScalarApiReference();
}

app.UseAzureAppConfiguration();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await app.SeedDatabaseAsync();

await app.RunAsync();
