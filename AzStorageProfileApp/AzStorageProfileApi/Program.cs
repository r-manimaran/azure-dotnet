using AzStorageProfileApi;
using AzStorageProfileApi.Extensions;
using AzStorageProfileApi.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOptions<AzureBlobStorageSettings>().BindConfiguration(AzureBlobStorageSettings.SectionName);
    //.ValidateDataAnnotations()
    //.ValidateOnStart();

builder.Services.AddControllers();

builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddScoped<IUserProfileService, UserProfileService>();

builder.Services.AddScoped<IBlobStorageService, BlobStorageService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
app.MapOpenApi();
app.UseSwaggerUI(opt =>
opt.SwaggerEndpoint("/openapi/v1.json", "Openapi v1"));
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.ApplyMigration();

app.Run();
