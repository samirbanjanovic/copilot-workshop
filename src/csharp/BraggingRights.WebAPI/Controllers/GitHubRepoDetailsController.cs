using Microsoft.AspNetCore.Mvc;

namespace BraggingRights.WebAPI
{
    [ApiController]
    public class GitHubRepoDetailsController : ControllerBase
    {
        private readonly GitHubApiService _gitHubApiService;

        public GitHubRepoDetailsController(GitHubApiService gitHubApiService)
        {
            _gitHubApiService = gitHubApiService;
        }

        /// <summary>
        /// This endpoint retrieves the details of GitHub repositories for a given user.
        /// </summary>
        /// <param name="username">The GitHub username.</param>
        /// <returns>
        /// A list of repositories belonging to the user, each with its associated programming languages.
        /// The repositories are sorted by the total bytes of code written in each language, in descending order.
        /// Repositories without any languages are filtered out.
        /// </returns>
        [HttpGet("api/githubrepodetails/{username}", Name = "GetGitHubRepoDetails")]
        public async Task<IActionResult> Get(string username)
        {
            var repos = await _gitHubApiService.GetUserRepos(username);

            var tasks = repos.Select((repo, index) => _gitHubApiService.GetRepoLanguages(username, repo.Name)
                .ContinueWith(t => (Index: index, Languages: t.Result)));

            var results = await Task.WhenAll(tasks);

            // for every repo add the language information
            foreach (var (index, languages) in results)
            {
                repos[index].Languages = languages;
            }

            var filteredRepos = repos.Where(repo => repo.Languages.Any()).ToList();

            return Ok(filteredRepos);
        }

        [HttpGet("api/githubrepodetails/{username}/{repoName}/workflows", Name = "GetGitHubRepoWorkflows")]
        public async Task<IActionResult> GetWorkflows(string username, string repoName)
        {
            var workflows = await _gitHubApiService.GetRepoWorkflows(username, repoName);
            return Ok(workflows);
        }
        
    }
}