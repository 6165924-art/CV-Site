using CvSite.Service;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CvSite.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SearchController : ControllerBase
    {
        private readonly IGitHubService _gitHubService;

        public SearchController(IGitHubService gitHubService)
        {
            _gitHubService = gitHubService;
        }

        [HttpGet]
        public async Task<IActionResult> Search([FromQuery] string? name, [FromQuery] string? language, [FromQuery] string? userName)
        {
            var repositories = await _gitHubService.SearchRepositories(name, language, userName);
            return Ok(repositories);
        }
    }
}
