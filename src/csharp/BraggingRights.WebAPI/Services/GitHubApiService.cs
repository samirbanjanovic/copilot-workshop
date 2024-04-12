using System.Text.Json;
using System.Text.Json.Serialization;

namespace BraggingRights.WebAPI;

/*

GitHub API Service that is responsible for connecting to the GitHub API and retrieving the data.
It has two operations:

1. One to get the users repositories
2. Get language details on each a users repostiories
*/
public class GitHubApiService
{
    private HttpClient _httpClient { get; }

    public GitHubApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<Repository>> GetUserRepos(string username)
    {
        // _httpClient.DefaultRequestHeaders.Add("User-Agent", "BraggingRights.WebAPI");
        var response = await _httpClient.GetAsync($"https://api.github.com/users/{username}/repos");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var repos = JsonSerializer.Deserialize<List<Repository>>(content, options);

        return repos;
    }

    public async Task<List<Language>> GetRepoLanguages(string username, string repoName)
    {
        var response = await _httpClient.GetAsync($"https://api.github.com/repos/{username}/{repoName}/languages");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var languages = JsonSerializer.Deserialize<Dictionary<string, int>>(content, options);

        return languages.Select(x => new Language { Name = x.Key, Bytes = x.Value }).ToList();
    }

    // create function to get workflows for a given repo
    public async Task<List<Workflow>> GetRepoWorkflows(string username, string repoName)
    {
        var response = await _httpClient.GetAsync($"https://api.github.com/repos/{username}/{repoName}/actions/workflows");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        var workflows = JsonSerializer.Deserialize<WorkflowResponse>(content, options);
        return workflows.Workflows;
    }


    public class WorkflowResponse
    {
        public int TotalCount { get; set; }
        public List<Workflow> Workflows { get; set; }
    }

    public class Workflow
    {
        public int Id { get; set; }
        public string NodeId { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string State { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public string Url { get; set; }
        public string HtmlUrl { get; set; }
        public string BadgeUrl { get; set; }
    }
}
