namespace WeatherApi.Services;

public class GitHubService
{
    private readonly ILogger<GitHubService> _logger;
    public HttpClient _httpClient;
    public GitHubService(IHttpClientFactory httpClientFactory, ILogger<GitHubService> logger)
    {
        _httpClient = httpClientFactory.CreateClient("GithubClient");
        _logger = logger;
    }
    public async Task<object?> GetUserDetails(string apiKey)
    {
        
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
        try
        {
            var response = await _httpClient.GetAsync("user");

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadFromJsonAsync<object>();

            return content;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());

            return null;
        }
    }

    public async Task<object?> GetUserRepositories(string apiKey)
    {
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
        try
        {
            var response = await _httpClient.GetAsync("user/repos?per_page=100&sort=created&direction=desc");

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadFromJsonAsync<object>();

            return content;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());

            return null;
        }
    }

    public async Task<object?> GetFolders(string owner, string repo, string apiKey, string path)
    {
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
        try
        {
            var response = await _httpClient.GetAsync($"repos/{owner}/{repo}/contents/{path}");

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadFromJsonAsync<object>();

            return content;

        }
        catch(Exception ex)
        {
            _logger.LogError(ex.ToString());

            return null;
        }
    }
}
