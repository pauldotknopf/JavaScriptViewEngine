using Sample.Mvc5.ViewModels;
using System;
using System.Web.Mvc;

namespace Sample.Mvc5.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(string greeting = "Hello word!")
        {
            ViewBag.propertyOnViewBag = "This is set in the controller";
            ViewBag.currentDate = DateTime.Now;
            return View("js-{auto}", new GreetingViewModel { Greeting = greeting });
        }
    }
}