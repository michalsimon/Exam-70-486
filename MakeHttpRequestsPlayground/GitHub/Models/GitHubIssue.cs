namespace MakeHttpRequestsPlayground.GitHub.Models
{
    using System;
    using Newtonsoft.Json;

    public class GitHubIssue
    {
        [JsonProperty(PropertyName = "html_url")]
        public string Url { get; set; }

        public string Title { get; set; }

        [JsonProperty(PropertyName = "created_at")]
        public DateTime Created { get; set; }
    }
}