var builder = DistributedApplication.CreateBuilder(args);

var applicationInsightsConnectionString =
        builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"];

builder.AddAzureFunctionsProject<Projects.FunQueueTrigger>("funqueuetrigger")
        .WithEnvironment("APPLICATIONINSIGHTS_CONNECTION_STRING", applicationInsightsConnectionString);

builder.Build().Run();
