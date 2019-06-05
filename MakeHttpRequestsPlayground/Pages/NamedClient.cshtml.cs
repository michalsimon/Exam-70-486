namespace MakeHttpRequestsPlayground.Pages
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using MakeHttpRequestsPlayground.GitHub.Models;
    using Microsoft.AspNetCore.Mvc.RazorPages;

    public class NamedClientModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly string GitHubPullRequestUri = "repos/aspnet/AspNetCore.Docs/pulls";

        public IEnumerable<GitHubPullRequest> PullRequests { get; private set; }

        public bool GetPullRequestsError { get; private set; }

        public bool HasPullRequests => PullRequests.Any();

        public NamedClientModel(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task OnGet()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, GitHubPullRequestUri);
            var client = _clientFactory.CreateClient("github");

            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                PullRequests = await response.Content.ReadAsAsync<IEnumerable<GitHubPullRequest>>();
            }
            else
            {
                GetPullRequestsError = true;
                PullRequests = Array.Empty<GitHubPullRequest>();
            }
        }
    }
}