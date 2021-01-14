using Microsoft.AspNetCore.Mvc;

namespace Nuka.MVC.Web.Controllers
{
    public class SampleController : Controller
    {
        // GET
        public IActionResult Index()
        {
            return View();
        }
    }
}