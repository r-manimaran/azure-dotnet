namespace WeatherWebApp;

public class WeatherService
{
    private readonly HttpClient _httpClient;

    public WeatherService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("WeatherClient");
    }
    
    public async Task<List<WeatherForecast>> GetWeatherData()
    {
        try
        {
            var response = await _httpClient.GetAsync("weatherforecast");
            
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadFromJsonAsync<List<WeatherForecast>>();

            return content;
        }
        catch (Exception ex)
        {
            return new List<WeatherForecast>();
        }
    }
}
