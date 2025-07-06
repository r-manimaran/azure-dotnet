using Aspire.Hosting.Azure;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.DependencyInjection;

namespace Aspire_ServiceBus_Topic.AppHost
{
    public static class ServiceBusExtensions
    {
        public static IResourceBuilder<AzureServiceBusTopicResource> WithTestCommands(
            this IResourceBuilder<AzureServiceBusTopicResource> builder)
        {
            builder.ApplicationBuilder.Services.AddSingleton<ServiceBusClient>(sp =>
            {
                var connectionString = builder.Resource.Parent.ConnectionStringExpression
                            .GetValueAsync(CancellationToken.None).GetAwaiter().GetResult();
                return new ServiceBusClient(connectionString);
            });

            builder.WithCommand("SendSbMessage", "Send message to Service Bus topic", executeCommand:
                async (sp) =>
                {
                    var client = sp.ServiceProvider.GetRequiredService<ServiceBusClient>();
                    var sender = client.CreateSender(builder.Resource.TopicName);       
                    
                    await sender.SendMessageAsync(new ServiceBusMessage("Hello World message!"));
                    Console.WriteLine($"Message sent to topic {builder.Resource.Name}.");
                    return new ExecuteCommandResult
                    {
                        Success = true                       
                    };
                });

            return builder;
        }
    }
}
