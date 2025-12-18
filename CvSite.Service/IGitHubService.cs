using CvSite.Service.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CvSite.Service
{
    public interface IGitHubService
    {
        Task<IEnumerable<RepositoryDto>> GetPortfolio();
        Task<IEnumerable<RepositoryDto>> SearchRepositories(string? name, string? language, string? userName);
    }
}
