using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using GlobalizationLocalization.Resources;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace GlobalizationLocalization.Controllers
{
    public class HomeController : Controller
    {
        private readonly IStringLocalizer _localizer;

        //public HomeController(IStringLocalizer<HomeController> localizer)
        public HomeController(IStringLocalizerFactory factory)
        {
            //_localizer = localizer;
            _localizer = factory.Create(typeof(Welcome));
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            ViewData["Hello"] = _localizer["Hello"];
            return View();
        }

        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) });

            return LocalRedirect(returnUrl);
        }
    }
}
