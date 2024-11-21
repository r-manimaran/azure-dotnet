namespace AzAppConfigKeyVault;

public class WeatherConfiguration
{
    // This the prefix used in the Azure App configuration.
    // Defined the key-value pair key as "weather:Count"
    
    public const string prefixName = "weather";

    public int Count {get;set;}

}
