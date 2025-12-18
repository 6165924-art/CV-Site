namespace CvSite.Service.Dtos
{
    public class RepositoryDto
    {
        public string? Name { get; set; }
        public string? Url { get; set; }
        public string? HomepageUrl { get; set; }
        public DateTimeOffset LastPush { get; set; }
        public int StargazersCount { get; set; }
        public int PullRequestsCount { get; set; }
        public IReadOnlyList<string>? Languages { get; set; }
    }
}
