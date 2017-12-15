using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace AspectCoreDemo.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICustomService _service;

        public HomeController(ICustomService service)
        {
            _service = service;
        }

        public IActionResult Index()
        {
            _service.Call();
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
