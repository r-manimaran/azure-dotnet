using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp;

public class PaymentGateway
{
    private readonly ILogger<PaymentGateway> _logger;
    private readonly TelemetryClient _telemetryClient;

    public PaymentGateway(ILogger<PaymentGateway> logger, TelemetryClient telemetryClient)
    {
        _logger = logger;
        _telemetryClient = telemetryClient;
    }

    [Function("ProcessPayment")]
    public async Task Run([HttpTrigger(AuthorizationLevel.Function,"post")] HttpRequest req)
    {
        var correlationId = Guid.NewGuid().ToString();
        var amount = new Random().Next(50, 5000);
        using (_logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = correlationId }))
        {

            // 1. Log a Custom Event (Business Metric)
            _telemetryClient.TrackEvent("PaymentInitiated", new Dictionary<string, string>
            {
                ["Currency"] = "USD",
                ["Tier"] = amount > 1000 ? "HighValue" : "Retail"
            });

            // 2. Track a Custom Metric (Numerical Data)
            _telemetryClient.GetMetric("PaymentAmount").TrackValue(amount);

            // 3. Simulate a Dependency Call with manual timing
            var startTime = DateTime.UtcNow;
            var timer = System.Diagnostics.Stopwatch.StartNew();

            await Task.Delay(new Random().Next(100, 500)); // Simulate work

            timer.Stop();

            // 4. Advanced: Log a manual dependency (Useful for 3rd party APIs)
            var dep = new DependencyTelemetry("ExternalCardProcessor",
                "Authorize", "v1/charge", "200", startTime, timer.Elapsed, "200",true);
            _telemetryClient.TrackDependency(dep);

            if (amount > 4500)
            {
                _logger.LogWarning("Potential Fraud Detected for amount {Amount}", amount);
                _telemetryClient.TrackEvent("FraudAlertTriggered");
            }

            _logger.LogInformation("Payment processed successfully.");
        }
    }
}
