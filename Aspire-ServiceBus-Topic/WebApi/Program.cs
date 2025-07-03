using Azure.Messaging.ServiceBus;
using WebApi;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddAzureServiceBusClient("insurance");

builder.Services.AddHostedService<ConsumerService>();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.MapPost("/notify", async (ServiceBusClient client, string message) =>
{
    var sender = client.CreateSender("insurance");
    var messageBatch = await sender.CreateMessageBatchAsync();

    if (!messageBatch.TryAddMessage(new ServiceBusMessage(message)))
        throw new Exception("Message too large for batch.");

    await sender.SendMessagesAsync(messageBatch);
    Console.WriteLine("Message sent to topic.");
});

app.Run();

