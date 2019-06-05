namespace MakeHttpRequestsPlayground.GitHub.Services
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;
    using MakeHttpRequestsPlayground.GitHub.Models;

    /// <summary>
    /// Exposes methods to return GitHub API data
    /// </summary>
    public class GitHubService
    {
        private readonly HttpClient _client;
        private readonly string GitHubIssueUri =
            "/repos/aspnet/AspNetCore.Docs/issues?state=open&sort=created&direction=desc";

        public GitHubService(HttpClient client)
        {
            client.BaseAddress = new Uri("https://api.github.com/");
            // GitHub API versioning
            client.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3+json");
            // GitHub requires a user-agent
            client.DefaultRequestHeaders.Add("User-Agent", "HttpClientFactory-Sample");

            _client = client;
        }

        public async Task<IEnumerable<GitHubIssue>> GetAspNetDocsIssues()
        {
            var response = await _client.GetAsync(GitHubIssueUri);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsAsync<IEnumerable<GitHubIssue>>();

            return result;
        }
    }
}