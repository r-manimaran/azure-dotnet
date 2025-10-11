var builder = DistributedApplication.CreateBuilder(args);

var appConfig = builder.AddAzureAppConfiguration("app-config-emu")
                       .RunAsEmulator(e=>e.WithDataVolume());

builder.AddProject<Projects.WebApi>("webapi")
       .WithReference(appConfig);

builder.Build().Run();
