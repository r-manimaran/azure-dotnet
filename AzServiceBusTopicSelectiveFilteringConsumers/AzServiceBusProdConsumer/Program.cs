using AzServiceBusProdConsumer;
using AzServiceBusProdConsumer.Consumers;
using AzServiceBusProdConsumer.Exensions;
using AzServiceBusProdConsumer.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOptions<AzureServiceBusSettings>().BindConfiguration(AzureServiceBusSettings.SectionName);

builder.Services.AddSingleton<IAzServiceBusService, AzServiceBusService>();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddScoped<IEventService, EventService>();

builder.Services.AddHostedService<TopicConsumerService>();

builder.Services.AddHostedService<NotificationConsumer>();

builder.Services.AddControllers();

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    options.SwaggerEndpoint("/openapi/v1.json", "OpenApi v1"));
}

await app.ApplyMigrations();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
