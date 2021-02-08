using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Nuka.Core.Models;

namespace Nuka.MVC.Web.Controllers
{
    public class HttpBinController : Controller
    {
        [HttpGet]
        [Authorize]
        public IActionResult Headers()
        {
            var headers = HttpContext.Request.Headers.ToDictionary(header => header.Key, header => header.Value);
            return Json(headers);
        }

        [HttpGet]
        [Authorize]
        public IActionResult RequestContexts()
        {
            var requestContext = HttpContext.RequestServices.GetRequiredService<RequestContext>();
            return Json(requestContext);
        }
    }
}