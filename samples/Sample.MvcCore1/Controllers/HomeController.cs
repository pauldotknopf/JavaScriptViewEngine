using System;
using Microsoft.AspNetCore.Mvc;
using Sample.MvcCore1.ViewModels;

namespace Sample.MvcCore1.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index(string greeting = "Hello word!")
        {
            ViewBag.propertyOnViewBag = "This is set in the controller";
            ViewBag.currentDate = DateTime.Now;
            return View("js-{auto}", new GreetingViewModel { Greeting = greeting });
        }
    }
}
