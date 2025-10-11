# Azure App Configuration Emulator - Sample Configurations

This document provides sample configuration values that can be added to the Azure App Configuration Emulator to test the various use cases.

## Configuration Keys and Values

### Database Configuration
```
Key: ConnectionStrings:DefaultConnection
Value: Server=localhost;Database=MyApp;Trusted_Connection=true;

Key: Database:CommandTimeout
Value: 45

Key: Database:RetryCount
Value: 5
```

### Feature Flags
```
Key: Features:EnableNewUI
Value: true

Key: Features:EnableBetaFeatures
Value: false

Key: Features:MaxFileUploadSize
Value: 52428800
```

### API Settings
```
Key: Api:RateLimitPerMinute
Value: 150

Key: Api:EnableCaching
Value: true

Key: Api:CacheExpirationMinutes
Value: 30
```

### Environment Settings
```
Key: Environment
Value: Development

Key: Logging:LogLevel:Default
Value: Debug

Key: DetailedErrors
Value: true
```

### App Settings
```
Key: AppSettings:ApplicationName
Value: Azure App Config Demo

Key: AppSettings:Version
Value: 1.2.0

Key: AppSettings:MaxConcurrentUsers
Value: 1000

Key: AppSettings:EnableMetrics
Value: true
```

### Feature Management (Advanced)
```
Key: FeatureManagement:NewDashboard
Value: true

Key: FeatureManagement:BetaFeatures
Value: false

Key: FeatureManagement:AdvancedReporting
Value: true

Key: FeatureManagement:ExperimentalUI
Value: false
```

### Percentage-based Feature Flags
```
Key: FeatureManagement:BetaFeatures:EnabledFor:0:Name
Value: Percentage

Key: FeatureManagement:BetaFeatures:EnabledFor:0:Parameters:Value
Value: 25
```

## Use Cases Demonstrated

1. **Database Configuration Management**: Centralized database connection strings and settings
2. **Feature Flag Management**: Toggle features on/off without code deployment
3. **API Rate Limiting**: Dynamic API configuration for performance tuning
4. **Environment-specific Settings**: Different configurations per environment
5. **Real-time Configuration Updates**: Dynamic configuration refresh without restart
6. **Percentage-based Feature Rollouts**: Gradual feature rollout to users
7. **Structured Configuration Sections**: Organized configuration hierarchy
8. **Connection String Management**: Secure connection string storage
9. **Application Settings**: General application configuration
10. **Logging Configuration**: Dynamic log level management

## Testing Instructions

1. Start the Azure App Configuration Emulator through Aspire
2. Add the above configuration values to the emulator
3. Use the provided HTTP requests in `UseCases.http` to test each endpoint
4. Modify configuration values in the emulator and test dynamic refresh
5. Toggle feature flags and observe behavior changes