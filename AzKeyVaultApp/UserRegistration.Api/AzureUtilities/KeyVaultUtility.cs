using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Polly;

namespace UserRegistration.Api.AzureUtilities;

public class KeyVaultUtility
{
    private readonly KeyVaultSettings _keyVaultSettings;
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<KeyVaultUtility> _logger;
    private readonly SecretClient _secretClient;

    public KeyVaultUtility(IOptions<KeyVaultSettings> keyVaultSettings,
                           IMemoryCache memoryCache,
                           ILogger<KeyVaultUtility> logger)
    {
        var credential = new DefaultAzureCredential(new DefaultAzureCredentialOptions
        {
            Diagnostics =
                {
                    LoggedHeaderNames = {"x-ms-request-id"},
                    LoggedQueryParameters = {"api-version"},
                    IsLoggingContentEnabled = true
                }
        });
        _secretClient = new SecretClient(new Uri(keyVaultSettings.Value.KeyVaultUrl), credential);
        _keyVaultSettings = keyVaultSettings.Value;
        _memoryCache = memoryCache;
        _logger = logger;

       
    }
    public async Task<string> GetSecretAsync(string secretName)
    {
        try
        {
            if (_memoryCache.TryGetValue(secretName, out string cachedSecret))
            {
                _logger.LogInformation($"Secret {secretName} found in cache");
                return cachedSecret;
            }
            _logger.LogInformation($"Secret {secretName} not found in cache. Fetching from KeyVault");

            // Define the retry policy using Polly
            // This lambda function defines the delay between retries. It uses an exponential backoff strategy,
            // where the delay increases exponentially with each retry attempt.
            // For example, the delays will be 2^1, 2^2, and 2^3 seconds for the 1st, 2nd, and 3rd retries, respectively.

            var _retryPolicy = Policy.Handle<Exception>().WaitAndRetryAsync(3,
                                                   retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                                                   onRetry: (exception, retryCount) =>
                                                   {
                                                       _logger.LogError($"Error:{exception.Message}. Retry attempt:{retryCount}");
                                                   });

            KeyVaultSecret secret = await _retryPolicy.ExecuteAsync(() => _secretClient.GetSecretAsync(secretName));

            _memoryCache.Set(secretName, secret.Value, TimeSpan.FromMinutes(30));

            return secret.Value;
        }
        catch (Azure.RequestFailedException ex) when (ex.Status == 403)
        {
            _logger.LogError(ex, "Access denied to Key Vault. Please check the following.");
            _logger.LogError("1. Verify the application has the correct RBAC role assigned.");
            _logger.LogError("2. Check if the application identity has access to the Key Vault.");
            _logger.LogError($"3. Verify the Key Vault URL is correct.:{_keyVaultSettings.KeyVaultUrl}");
            throw new UnauthorizedAccessException("Access denied to Key Vault. Please check the logs for more details.", ex);
        }
        catch (Azure.RequestFailedException ex) when (ex.Status == 404)
        {
            _logger.LogError(ex, $"Secret {secretName} not found in Key Vault.");
            throw new KeyNotFoundException($"Secret {secretName} not found in Key Vault.", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while getting secret from KeyVault");
            throw;
        }
    }
}
