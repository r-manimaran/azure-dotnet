
using CosmosApi.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Polly;

namespace CosmosApi;

public class DatabaseBootstrapper : BackgroundService, IHealthCheck
{
    private readonly CosmosClient _cosmosClient;
    private readonly ILogger<DatabaseBootstrapper> _logger;

    private bool _dbCreated;
    private bool _dbCreationFailed;
    public DatabaseBootstrapper(CosmosClient cosmosClient, ILogger<DatabaseBootstrapper> logger)
    {
        _cosmosClient = cosmosClient;
        _logger = logger;
    }

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken=default)
    {
        var status = _dbCreated
            ? HealthCheckResult.Healthy()
            : _dbCreationFailed
                ? HealthCheckResult.Unhealthy("Database creation failed")
                : HealthCheckResult.Degraded("Database creation is still in progress.");
        return Task.FromResult(status);
              
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
       // The CosmosDB emulator can take a very long time to start. So use a custom resilience strategy to ensure
       // it retries long enough.

        var retry = new ResiliencePipelineBuilder()
                    .AddRetry(new()
                    {
                        Delay = TimeSpan.FromSeconds(5),
                        MaxRetryAttempts = 60,
                        BackoffType = DelayBackoffType.Constant,
                        OnRetry = args =>
                        {
                            _logger.LogWarning("""
                               Issue during database creation after {AttemptDuration} on attempt {AttemptNumber}. Will try in {RetryDelay}.
                               Exception: 
                                  {ExceptionMessage}
                                  {InnerExceptionMessage}
                               """,
                                args.Duration,
                                args.AttemptNumber,
                                args.RetryDelay,
                                args.Outcome.Exception?.Message ?? "[none]",
                                args.Outcome.Exception?.InnerException?.Message ?? "");
                            return ValueTask.CompletedTask;
                        }
                    }).Build();

        await retry.ExecuteAsync(async ct =>
        {
            await _cosmosClient.CreateDatabaseIfNotExistsAsync("iotdb", cancellationToken: ct);
            var database = _cosmosClient.GetDatabase("iotdb");
            await database.CreateContainerIfNotExistsAsync(new ContainerProperties("iots",IoTDevice.UserIdPartitionKey),
                cancellationToken: ct);
            _logger.LogInformation("Database successfully created!");
            _dbCreated = true;
        }, stoppingToken);

        _dbCreationFailed = !_dbCreated;
    }
}
