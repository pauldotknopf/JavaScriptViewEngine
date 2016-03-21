using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Sample.Mvc6.ViewModels;

namespace Sample.Mvc6.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index(string greeting = "Hello word!")
        {
            return View("js-{auto}", new GreetingViewModel { Greeting = greeting });
        }
    }
}
