using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailNotificatonTimer;

public static class AppExtensions
{
    public static ILoggingBuilder AddSerilogLogger(this ILoggingBuilder builder)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateLogger();
        builder.ClearProviders();
        builder.AddSerilog(Log.Logger, dispose: true); // dispose: true ensures that the logger is disposed when the application shuts down
        return builder;
    }
}
