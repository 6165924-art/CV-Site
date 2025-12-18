using CvSite.Service.Dtos;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CvSite.Service
{
    public class CachingGitHubService : IGitHubService
    {
        private readonly IGitHubService _decorated;
        private readonly IMemoryCache _cache;
        private readonly GitHubClient _gitHubClient;
        private readonly string _username;
        private const string PortfolioCacheKey = "Portfolio";

        public CachingGitHubService(IGitHubService decorated, IMemoryCache cache, IOptions<GitHubOptions> options)
        {
            _decorated = decorated;
            _cache = cache;
            _username = options.Value.Username;
            _gitHubClient = new GitHubClient(new ProductHeaderValue("CvSite"))
            {
                Credentials = new Credentials(options.Value.Token)
            };
        }

        public async Task<IEnumerable<RepositoryDto>> GetPortfolio()
        {
            if (_cache.TryGetValue(PortfolioCacheKey, out PortfolioCacheEntry? cachedEntry) && cachedEntry != null)
            {
                var userActivities = await _gitHubClient.Activity.Events.GetAllUserPerformed(_username);
                var lastActivityDate = userActivities.FirstOrDefault()?.CreatedAt ?? DateTimeOffset.MinValue;

                if (lastActivityDate > cachedEntry.LastFetched)
                {
                    return await FetchAndCachePortfolio();
                }
                
                return cachedEntry.Portfolio ?? Enumerable.Empty<RepositoryDto>();
            }
            
            return await FetchAndCachePortfolio();
        }

        private async Task<IEnumerable<RepositoryDto>> FetchAndCachePortfolio()
        {
            var portfolio = await _decorated.GetPortfolio();
            var newEntry = new PortfolioCacheEntry
            {
                Portfolio = portfolio,
                LastFetched = DateTimeOffset.UtcNow
            };
            
            _cache.Set(PortfolioCacheKey, newEntry);
            return portfolio;
        }

        public Task<IEnumerable<RepositoryDto>> SearchRepositories(string? name, string? language, string? userName)
        {
            return _decorated.SearchRepositories(name, language, userName);
        }
    }
}
