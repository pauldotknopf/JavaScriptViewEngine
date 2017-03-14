using Microsoft.AspNetCore.Mvc;

namespace Samples.MvcCore.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
			return View("js-{auto}");
        }
    }
}
