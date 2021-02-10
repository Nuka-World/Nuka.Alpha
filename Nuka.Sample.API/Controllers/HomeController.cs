using Microsoft.AspNetCore.Authorization;
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
        
        [Authorize]
        [HttpGet]
        public IActionResult Index()
        {
            SampleItem item = _service.GetItemById(1);

            if (item == null)
            {
                item = new SampleItem()
                {
                    ItemId = "00001",
                    ItemName = "Item01",
                    Description = "Desciption01",
                    Price = 10.01m,
                    SampleType = new SampleType() {Type = "Sample"}
                };
                _service.Insert(item);
            }

            return Json(item.ToModel<SampleItemModel>());
        }
    }
}