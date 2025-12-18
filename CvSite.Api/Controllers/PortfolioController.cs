using CvSite.Service;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CvSite.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PortfolioController : ControllerBase
    {
        private readonly IGitHubService _gitHubService;

        public PortfolioController(IGitHubService gitHubService)
        {
            _gitHubService = gitHubService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var portfolio = await _gitHubService.GetPortfolio();
            return Ok(portfolio);
        }
    }
}
