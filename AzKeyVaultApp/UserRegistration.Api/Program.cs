using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using UserRegistration.Api.AzureUtilities;
using UserRegistration.Api.Data;
using UserRegistration.Api.Extensions;
using UserRegistration.Api.Services;


var builder = WebApplication.CreateBuilder(args);


builder.Services.Configure<KeyVaultSettings>(builder.Configuration.GetSection("KeyVaultSettings"));
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<KeyVaultUtility>();
builder.Services.AddLogging();

var serviceProvider = builder.Services.BuildServiceProvider();
var keyVaultUtility = serviceProvider.GetRequiredService<KeyVaultUtility>();

// Fetch the connectionString from appSettings
// var sqlConnectionString = builder.Configuration.GetConnectionString("AzureSql");

// Fetch the connectionString from KeyVault
var sqlConnectionString = await keyVaultUtility.GetSecretAsync("AzureSqlDbConnectionString");

builder.Services.AddDbContext<AppDbContext>(o=>
    o.UseSqlServer(sqlConnectionString)
   );

//builder.Services.AddDbContext<AppDbContext>(options =>
//{
//    options.UseInMemoryDatabase("Users");
//});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IPasswordHashService, PasswordHashService>();

builder.Services.AddControllers();

builder.Services.AddOpenApi();

builder.Services.AddFluentEmail(
    builder.Configuration["Email:SenderEmail"], 
    builder.Configuration["Email:Sender"])
    .AddSmtpSender(
    builder.Configuration["Email:Host"], 
    builder.Configuration.GetValue<int>("Email:Port"));

// Services
builder.Services.AddScoped<IUserRegistrationService, UserRegistrationService>();
builder.Services.AddScoped<EmailVerificationLinkService>();
builder.Services.AddHttpContextAccessor();

// OpenTelemetry
builder.Logging.AddOpenTelemetry(logging =>
{
    logging.IncludeFormattedMessage = true;
    logging.IncludeScopes = true;
});

builder.Services.AddOpenTelemetry()

    .ConfigureResource(resource => resource.AddService(
        serviceName: builder.Environment.ApplicationName,
        serviceVersion: typeof(Program).Assembly.GetName().Version?.ToString() ?? "1.0.0"))

    .WithTracing(tracing =>
        tracing.AddHttpClientInstrumentation(options => 
        {
            options.RecordException = true;            
        })
        .AddAspNetCoreInstrumentation(options => 
        {
            options.RecordException = true;
            options.Filter = ctx => !ctx.Request.Path.StartsWithSegments("/health");
        })
        ).UseOtlpExporter()
    .WithMetrics(metrics =>
        metrics.AddRuntimeInstrumentation());
    
var app = builder.Build();

app.MapOpenApi();

app.UseSwagger();

app.UseSwaggerUI();

app.ApplyMigration();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
