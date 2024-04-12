using System.Net;
using System.Text;
using BraggingRights.WebAPI;
using Moq;

namespace BraggingRights.Tests;


public class GitHubApiServiceTests
{

    public class TestHttpMessageHandler : DelegatingHandler
    {
        private HttpResponseMessage _fakeResponse;

        public TestHttpMessageHandler(HttpResponseMessage responseMessage)
        {
            _fakeResponse = responseMessage;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_fakeResponse);
        }
    }

    public GitHubApiServiceTests()
    { }

    [Fact]
    public async Task GetUserRepos_ReturnsListOfRepositories_WhenUserExists()
    {
        var repoName = "TestRepo";
        var repoDescription = "This is a test repo";
        var repoUrl = "http://github.com/testuser/testrepo";

        var responseString = $"[{{ \"name\": \"{repoName}\", \"description\": \"{repoDescription}\", \"htmlUrl\": \"{repoUrl}\" }}]";


        var messageHandler = ConfigureTestMessageHandler(HttpStatusCode.OK, responseString);
        var client = new HttpClient(messageHandler);
        var subject = new GitHubApiService(client);
        // Act
        var result = await subject.GetUserRepos("testuser");

        // Assert
        Assert.Single(result);
        Assert.Equal(repoName, result[0].Name);
        Assert.Equal(repoDescription, result[0].Description);    
        Assert.Equal(repoUrl, result[0].HtmlUrl);
    }

    [Fact]
    public async Task GetUserRepos_ThrowsHttpRequestException_WhenUserDoesNotExist()
    {
        // Arrange
        var messageHandler = ConfigureTestMessageHandler(HttpStatusCode.NotFound, "");
        var client = new HttpClient(messageHandler);

        var gitHubApiService = new GitHubApiService(client);

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(() => gitHubApiService.GetUserRepos("nonexistentuser"));
    }

    [Fact]
    public async Task GetRepoLanguages_ReturnsListOfLanguages_WhenRepoExists()
    {
        // Arrange
        var messageHandler = ConfigureTestMessageHandler(HttpStatusCode.OK, "{\"C#\": 10000, \"JavaScript\": 5000}");
        var client = new HttpClient(messageHandler);
        var gitHubApiService = new GitHubApiService(client);

        // Act
        var result = await gitHubApiService.GetRepoLanguages("testuser", "testrepo");

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(result, x => x.Name == "C#" && x.Bytes == 10000);
        Assert.Contains(result, x => x.Name == "JavaScript" && x.Bytes == 5000);
    }

    [Fact]
    public async Task GetRepoLanguages_ThrowsHttpRequestException_WhenRepoDoesNotExist()
    {
        // Arrange
        var messageHandler = ConfigureTestMessageHandler(HttpStatusCode.NotFound, "");
        var client = new HttpClient(messageHandler);
        var gitHubApiService = new GitHubApiService(client);

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(() => gitHubApiService.GetRepoLanguages("testuser", "nonexistentrepo"));
    }

    // create test for GetRepoWorkflows
    [Fact]
    public async Task GetRepoWorkflows_ReturnsListOfWorkflows_WhenRepoExists()
    {
        // Arrange
        var messageHandler = ConfigureTestMessageHandler(HttpStatusCode.OK, "{\"total_count\": 1, \"workflows\": [{\"name\": \"testworkflow\", \"path\": \".github/workflows/testworkflow.yml\"}]}");
        var client = new HttpClient(messageHandler);
        var gitHubApiService = new GitHubApiService(client);
        // Act
        var result = await gitHubApiService.GetRepoWorkflows("testuser", "testrepo");
        // Assert
        Assert.Single(result);
        Assert.Equal("testworkflow", result[0].Name);
        Assert.Equal(".github/workflows/testworkflow.yml", result[0].Path);
    }

    // test when repo does not exist
    [Fact]
    public async Task GetRepoWorkflows_ThrowsHttpRequestException_WhenRepoDoesNotExist()
    {
        // Arrange
        var messageHandler = ConfigureTestMessageHandler(HttpStatusCode.NotFound, "");
        var client = new HttpClient(messageHandler);
        var gitHubApiService = new GitHubApiService(client);
        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(() => gitHubApiService.GetRepoWorkflows("testuser", "nonexistentrepo"));
    }

    // test when api returns 401
    [Fact]
    public async Task GetRepoWorkflows_ThrowsHttpRequestException_WhenUnauthorized()
    {
        // Arrange
        var messageHandler = ConfigureTestMessageHandler(HttpStatusCode.Unauthorized, "");
        var client = new HttpClient(messageHandler);
        var gitHubApiService = new GitHubApiService(client);
        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(() => gitHubApiService.GetRepoWorkflows("testuser", "testrepo"));
    }

    #region private helper methods

    private TestHttpMessageHandler ConfigureTestMessageHandler(HttpStatusCode statusCode, string content)
    {
        var fakeHttpResponse = new HttpResponseMessage
        {
            StatusCode = statusCode,
            Content = new StringContent(content, Encoding.UTF8, "application/json")
        };

        return new TestHttpMessageHandler(fakeHttpResponse);
    }


    #endregion
}