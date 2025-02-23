var builder = DistributedApplication.CreateBuilder(args);

var connectionString = builder.AddConnectionString("Postgres");

builder.AddProject<Projects.EcommerceApp_Host>("product-service")
       .WithExternalHttpEndpoints()
       .WithReference(connectionString);

builder.Build().Run();
