using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var builder = DistributedApplication.CreateBuilder(args);

var logger = builder.Services.BuildServiceProvider()
                             .GetRequiredService<ILogger<Program>>();
// icons location url
https://github.com/microsoft/fluentui-system-icons/blob/main/icons_filled.md
var appConfig = builder.AddAzureAppConfiguration("app-config-emu")
                       .RunAsEmulator(e=>e.WithDataVolume())
                       .WithIconName("VehicleTruckBag")
                       .OnResourceStopped((r,e,ct)=>
                       {
                           var state = e.ResourceEvent.Snapshot.State?.Text;

                           var t = e.ResourceEvent.Snapshot.StopTimeStamp;
                           logger.LogInformation("Resource: {Resource} state: {State} at {Time}", r.Name,state, t);
                           logger.LogInformation("App Configuration Emulator has stopped");

                            return Task.CompletedTask;
                       });

builder.AddProject<Projects.WebApi>("webapi")
       .WithReference(appConfig)
       .WithIconName("VehicleCarCollision");

builder.Build().Run();
