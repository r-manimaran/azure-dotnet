using WeatherApi.Services;

namespace WeatherApi.Endpoints;

public static class GithubEndpoints
{
    public static void MapGitHubEndpoints(this IEndpointRouteBuilder builder)
    {
        var api = builder.MapGroup("/api/Github").WithTags("GitHub");

        api.MapGet("/user", async (string apiKey, GitHubService githubService) =>
        {
            var response = await githubService.GetUserDetails(apiKey);

            return Results.Ok(response);
        });

        api.MapGet("/repositories", async (string apiKey, GitHubService githubService) =>
        {
            var res = await githubService.GetUserRepositories(apiKey);

            return Results.Ok(res);
        });

        api.MapGet("/folders/{owner}/{repo}", async (string owner, string repo, string apiKey, GitHubService githubService, string path = "") =>
        {
            var res = await githubService.GetFolders(owner, repo, apiKey, path);

            return Results.Ok(res);
        });
    }
}
