using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nuka.Sample.HttpAggregator.Models;
using Nuka.Sample.HttpAggregator.Services;

namespace Nuka.Sample.HttpAggregator.Controllers
{
    public class SampleController : Controller
    {
        private readonly SampleService _service;
        private readonly ILogger _logger;
        private readonly HttpContext _httpContext;

        public SampleController(
            SampleService service,
            IHttpContextAccessor httpContextAccessor,
            ILogger<SampleController> logger)
        {
            _service = service;
            _logger = logger;
            _httpContext = httpContextAccessor.HttpContext;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<SampleItemModel>> Item([FromRoute] int id)
        {
            var result = await _service.GetItemById(id);
            return result;
        }
    }
}