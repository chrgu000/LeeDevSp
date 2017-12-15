using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AuthenticationTest.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            returnUrl = returnUrl ?? (ViewData["ReturnUrl"] ?? "/Account/Index").ToString();
            ViewData["ReturnUrl"] = returnUrl;
            if (User.Identity.IsAuthenticated)
            {
                return RedirectPermanent(returnUrl);
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string userName, string password, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (!string.IsNullOrEmpty(userName) && userName == password)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name,userName),
                    new Claim("password",password),
                    new Claim("realname","Jackie Lee")
                };
                // init the identity instance
                var userPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims, "Customer"));
                // signin
                await HttpContext.Authentication.SignInAsync("CookieAuth", userPrincipal, new Microsoft.AspNetCore.Http.Authentication.AuthenticationProperties
                {
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(20),
                    IsPersistent = false,
                    AllowRefresh = false
                });
                return RedirectPermanent(returnUrl);
            }
            else
            {
                ViewBag.ErrMsg = "UserName or Password is invalid";
                return View();
            }
        }

        [HttpPost]
        public string GetCurrUserRealname()
        {
            var str = User.Identities.FirstOrDefault(u => u.IsAuthenticated).FindFirst("realname").Value;
            return $"{User.Identity.Name}:{str}";
        }

        public async Task<JsonResult> Logout()
        {
            await HttpContext.Authentication.SignOutAsync("CookieAuth");
            return Json(new { data = "验证方案：CookieAuth成功退出" });
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }
    }
}
