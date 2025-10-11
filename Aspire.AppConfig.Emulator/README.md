# Azure App Configuration Emulator Use Cases

This project demonstrates various use cases for Azure App Configuration Emulator with .NET Aspire.

## Use Cases Implemented

### 1. Database Configuration Management
- **Endpoint**: `GET /api/config/database`
- **Purpose**: Centralized database connection strings and timeout settings
- **Configuration Keys**:
  - `ConnectionStrings:DefaultConnection`
  - `Database:CommandTimeout`
  - `Database:RetryCount`

### 2. Feature Flag Management
- **Endpoint**: `GET /api/config/features`
- **Purpose**: Toggle application features without code deployment
- **Configuration Keys**:
  - `Features:EnableNewUI`
  - `Features:EnableBetaFeatures`
  - `Features:MaxFileUploadSize`

### 3. API Settings Configuration
- **Endpoint**: `GET /api/config/api-settings`
- **Purpose**: Dynamic API behavior configuration
- **Configuration Keys**:
  - `Api:RateLimitPerMinute`
  - `Api:EnableCaching`
  - `Api:CacheExpirationMinutes`

### 4. Environment-Specific Settings
- **Endpoint**: `GET /api/config/environment`
- **Purpose**: Environment-specific configuration management
- **Configuration Keys**:
  - `Environment`
  - `Logging:LogLevel:Default`
  - `DetailedErrors`

### 5. Advanced Feature Flag Management
- **Endpoints**: 
  - `GET /api/featureflag/check/{featureName}`
  - `GET /api/featureflag/all`
  - `GET /api/featureflag/percentage/{featureName}`
- **Purpose**: Advanced feature management with percentage rollouts
- **Uses**: Microsoft.FeatureManagement library

### 6. Dynamic Configuration Updates
- **Endpoints**:
  - `GET /api/dynamicconfig/refresh`
  - `GET /api/dynamicconfig/app-settings`
  - `GET /api/dynamicconfig/connection-strings`
  - `GET /api/dynamicconfig/section/{sectionName}`
- **Purpose**: Real-time configuration updates without application restart

## Getting Started

1. **Run the Application**:
   ```bash
   dotnet run --project Aspire.AppConfig.Emulator.AppHost
   ```

2. **Access the Aspire Dashboard**: Navigate to the provided URL to see the running services

3. **Configure the Emulator**: Add configuration values through the Azure App Configuration Emulator interface

4. **Test the APIs**: Use the provided `UseCases.http` file to test all endpoints

## Sample Configuration Values

Refer to `SampleConfigurations.md` for complete sample configuration values to add to the emulator.

## Key Features Demonstrated

- **Centralized Configuration**: All application settings in one place
- **Feature Toggles**: Enable/disable features without deployment
- **Environment Management**: Different configs per environment
- **Dynamic Updates**: Configuration changes without restart
- **Percentage Rollouts**: Gradual feature deployment
- **Structured Configuration**: Hierarchical configuration organization
- **Security**: Secure connection string management

## Architecture

- **AppHost**: Aspire orchestration with Azure App Configuration Emulator
- **WebApi**: ASP.NET Core API demonstrating configuration usage
- **Controllers**: Separate controllers for different configuration scenarios
- **Feature Management**: Integration with Microsoft.FeatureManagement

## Testing

Use the `UseCases.http` file with your favorite HTTP client to test all endpoints and scenarios.