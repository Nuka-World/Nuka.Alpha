using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nuka.Core.Constants;
using Nuka.Core.Mappers;
using Nuka.Core.Messaging;
using Nuka.Core.Models;
using Nuka.Sample.API.Data.Entities;
using Nuka.Sample.API.Messaging.EventPublish;
using Nuka.Sample.API.Models;
using Nuka.Sample.API.Services;

namespace Nuka.Sample.API.Controllers
{
    public class HomeController : Controller
    {
        private readonly SampleService _service;
        private readonly RequestContext _requestContext;
        private readonly IEventPublisher _eventPublisher;
        private readonly ILogger<HomeController> _logger;
        
        public HomeController(
            SampleService service,
            RequestContext requestContext,
            ILogger<HomeController> logger,
            IEventPublisher eventPublisher = null)
        {
            _logger = logger;
            _service = service;
            _requestContext = requestContext;
            _eventPublisher = eventPublisher;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Index()
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

            // Publish Event
            if (_eventPublisher != null)
                await _eventPublisher.PublishAsync(new SampleEvent
                {
                    CorrelationId = _requestContext[StandardHeaders.CorrelationId],
                    ItemId = "00001"
                });

            return Json(item.ToModel<SampleItemModel>());
        }

        [HttpGet]
        public Task<IActionResult> Error()
        {
            throw new Exception("This is Demo Exception");
        }
    }
}