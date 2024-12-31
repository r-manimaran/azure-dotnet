var builder = DistributedApplication.CreateBuilder(args);

var cosmosdb = builder.AddAzureCosmosDB("cosmos").AddDatabase("iotdb")
                       .RunAsEmulator(); //Remove this to use a live instance 

builder.AddProject<Projects.CosmosApi>("cosmosapi")
       .WithReference(cosmosdb)
       .WaitFor(cosmosdb);
        
builder.Build().Run();
