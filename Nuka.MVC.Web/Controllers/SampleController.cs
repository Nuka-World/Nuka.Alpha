using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nuka.MVC.Web.Services;

namespace Nuka.MVC.Web.Controllers
{
    public class SampleController : Controller
    {
        private readonly SampleService _service;
        private readonly ILogger _logger;

        public SampleController(
            SampleService service,
            ILogger<SampleController> logger)
        {
            _service = service;
            _logger = logger;
        }
        
        [HttpGet]
        public IActionResult Index()
        {
            return Json("Hello World!");
        }
        
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Item([FromRoute] int id)
        {
            var model = await _service.GetSampleItemByIdAsync(id);

            return View(model);
        }
    }
}