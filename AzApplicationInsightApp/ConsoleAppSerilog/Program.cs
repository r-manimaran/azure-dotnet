using Microsoft.ApplicationInsights.Extensibility;
using Serilog;

using var log = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("log.txt")
    .WriteTo.ApplicationInsights(new TelemetryConfiguration
    {
        InstrumentationKey = "30cc70c7-d0ad-4406-9a54-984b4d5fd9c6"
    }, TelemetryConverter.Traces).CreateLogger();

int i = 0;
while(true)
{
    log.Information($"Hello From Serilog! {++i}");
    Thread.Sleep(1000);
}