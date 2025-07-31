using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureUtilities;

public class ServicePrefixKeyVaultSecretManager : KeyVaultSecretManager
{
    private readonly string _servicePrefix;
    private readonly ILogger<ServicePrefixKeyVaultSecretManager> _logger;
    private readonly string _fallbackPrefix;
    private readonly HashSet<string> _validPrefixes;
    public ServicePrefixKeyVaultSecretManager(string servicePrefix, string prefixDelimeter,
        string validPrefixes, 
        ILogger<ServicePrefixKeyVaultSecretManager> logger,
        string fallbackPrefix="")
    {
        _servicePrefix = $"{servicePrefix}{prefixDelimeter}";
        _logger = logger;
        _fallbackPrefix = fallbackPrefix ;
        _validPrefixes = new HashSet<string>(validPrefixes.Split(',', StringSplitOptions.RemoveEmptyEntries),
            StringComparer.OrdinalIgnoreCase);

        // Validate the Service prefix at construction
        if (!_validPrefixes.Contains(servicePrefix))
        {
            throw new InvalidOperationException(
                $"Invalid service prefix '{servicePrefix}'. Valid prefixes:{string.Join(",", _validPrefixes)}");
        }
    }

    public override bool Load(SecretProperties secret)
    {
        // Load secrets that match the service prefix or are fallback secrets
        bool isServiceSecret = secret.Name.StartsWith(_servicePrefix, StringComparison.OrdinalIgnoreCase);
        bool isFallbackSecret = !string.IsNullOrEmpty(_fallbackPrefix)  &&
                                secret.Name.StartsWith(_fallbackPrefix, StringComparison.OrdinalIgnoreCase);

        if(isServiceSecret || isFallbackSecret)
        {
            _logger.LogInformation("Loading secret:{SecretName}",secret.Name);
            return true;
        }
        _logger.LogInformation("Skipping Secret:{SecretName} (does not match prefix)", secret.Name);
        return false;
    }
    public override string GetKey(KeyVaultSecret secret)
    {
        // Remove service prefix or fallback prefix from secret name
        string key = secret.Name;
        if(key.StartsWith(_servicePrefix, StringComparison.OrdinalIgnoreCase))
        {
            key = key.Substring(_servicePrefix.Length);
        }
        else if(!string.IsNullOrEmpty(_fallbackPrefix) &&
            key.StartsWith(_fallbackPrefix, StringComparison.OrdinalIgnoreCase))
        {
            key = key.Substring(_fallbackPrefix.Length);
            _logger.LogWarning("Using fallback secret: {SecretName}", secret.Name);
        }
        return key.Replace("--", ":");
    }

    /// <summary>
    /// Validate that requried secrets are present
    /// </summary>
    /// <param name="secrets"></param>

    public void ValidateSecrets(IEnumerable<KeyVaultSecret> secrets, params string[] requiredKeys)
    {
        var loadedSecretNames = secrets.Select(s => s.Name).ToList();
        var expectedSecrets = requiredKeys.Select(k => k.StartsWith(_servicePrefix) ? k : $"{_servicePrefix}{k}").ToList();
        var missingSecrets = expectedSecrets.Except(loadedSecretNames).ToArray();
        if (missingSecrets.Any())
        {
            throw new InvalidOperationException($"Missing secrets:{string.Join(", ", missingSecrets)}");
        }

        // Warn if unexpected secrets are loaded.
        var unexpectedSecrets = loadedSecretNames
            .Where(n=>!n.StartsWith(_servicePrefix,StringComparison.OrdinalIgnoreCase) && 
                    !n.StartsWith(_fallbackPrefix,StringComparison.OrdinalIgnoreCase) &&
                    !n.Equals("ServicePrefixes",StringComparison.OrdinalIgnoreCase))
            .ToArray();
        if (unexpectedSecrets.Any())
        {
            _logger.LogWarning("Unexpected secrets loaded:{Secrets}",string.Join(", ", unexpectedSecrets));
        }
    }
}
