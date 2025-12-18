using CvSite.Service.Dtos;
using Microsoft.Extensions.Options;
using Octokit;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CvSite.Service
{
    public class GitHubService : IGitHubService
    {
        private readonly GitHubClient _gitHubClient;
        private readonly GitHubOptions _options;

        public GitHubService(IOptions<GitHubOptions> options)
        {
            _options = options.Value;
            _gitHubClient = new GitHubClient(new ProductHeaderValue("CvSite"))
            {
                Credentials = new Credentials(_options.Token)
            };
        }

        public async Task<IEnumerable<RepositoryDto>> GetPortfolio()
        {
            var repositories = await _gitHubClient.Repository.GetAllForCurrent();
            var portfolio = new List<RepositoryDto>();

            foreach (var repo in repositories)
            {
                var languages = await _gitHubClient.Repository.GetAllLanguages(repo.Id);
                var pullRequests = await _gitHubClient.PullRequest.GetAllForRepository(repo.Id);

                portfolio.Add(new RepositoryDto
                {
                    Name = repo.Name,
                    Url = repo.HtmlUrl,
                    HomepageUrl = repo.Homepage,
                    LastPush = repo.PushedAt ?? repo.UpdatedAt,
                    StargazersCount = repo.StargazersCount,
                    PullRequestsCount = pullRequests.Count,
                    Languages = languages.Select(l => l.Name).ToList()
                });
            }

            return portfolio;
        }

        public async Task<IEnumerable<RepositoryDto>> SearchRepositories(string? name, string? language, string? userName)
        {
            var query = new StringBuilder();

            if (!string.IsNullOrEmpty(name))
            {
                query.Append(name + " in:name ");
            }
            if (!string.IsNullOrEmpty(language))
            {
                query.Append($"language:{language} ");
            }
            if (!string.IsNullOrEmpty(userName))
            {
                query.Append($"user:{userName} ");
            }

            var finalQuery = query.ToString().Trim();
            if (string.IsNullOrEmpty(finalQuery))
            {
                return Enumerable.Empty<RepositoryDto>();
            }
            
            var unauthenticatedClient = new GitHubClient(new ProductHeaderValue("CvSite"));
            var searchRequest = new SearchRepositoriesRequest(finalQuery);
            var searchResult = await unauthenticatedClient.Search.SearchRepo(searchRequest);

            return searchResult.Items.Select(repo => new RepositoryDto
            {
                Name = repo.Name,
                Url = repo.HtmlUrl,
                HomepageUrl = repo.Homepage,
                LastPush = repo.PushedAt ?? repo.UpdatedAt,
                StargazersCount = repo.StargazersCount,
            }).ToList();
        }
    }
}
