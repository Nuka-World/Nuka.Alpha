using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nuka.Core.Mappers;
using Nuka.Sample.API.Data.Entities;
using Nuka.Sample.API.Models;
using Nuka.Sample.API.Services;

namespace Nuka.Sample.API.Controllers
{
    public class HomeController : Controller
    {
        private readonly SampleService _service;
        private readonly ILogger<HomeController> _logger;

        public HomeController(
            SampleService service,
            ILogger<HomeController> logger)
        {
            _logger = logger;
            _service = service;
        }

        [HttpGet]
        public IActionResult Index()
        {
            _logger.LogInformation("Hello World!");

            var @type = new SampleType() {Type = "Sample"};

            var item = new SampleItem()
            {
                Name = "Item01",
                Description = "Desciption01",
                Price = 10.01m,
                SampleType = @type
            };
            
            _service.Insert(item);

            return Json(item.ToModel<SampleItemModel>());
        }
    }
}