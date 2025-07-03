using Aspire.Hosting.Azure;

var builder = DistributedApplication.CreateBuilder(args);

var serviceBus = builder.AddAzureServiceBus("sbemulator").RunAsEmulator();
// add topic and subscription
var topic = serviceBus.AddServiceBusTopic("insurance");
topic.AddServiceBusSubscription("sub1")
    .WithProperties(subscription =>
    {
        subscription.MaxDeliveryCount = 3;
        /*subscription.Rules.Add(new AzureServiceBusRule("filter-rule")
        {
            CorrelationFilter = new()
            {
                ContentType = "application/json",
                CorrelationId = "id1"
            }
        });*/
    });
builder.AddProject<Projects.WebApi>("webapi").WithReference(topic).WaitFor(topic);

builder.AddProject<Projects.ConsoleConsumer>("consoleconsumer").WithReference(topic).WaitFor(topic);

builder.Build().Run();
