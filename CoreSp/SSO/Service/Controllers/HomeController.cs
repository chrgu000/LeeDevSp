using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Service.Models;
using Microsoft.AspNetCore.Http;

namespace Service.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
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
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private static IDictionary<string, Guid> TokenIds = new Dictionary<string, Guid>();

        private bool CheckAuthroize(string name, string password)
        {
            return name.Equals("lee", StringComparison.OrdinalIgnoreCase) &&
                password.Equals("123456");
        }

        public IActionResult Login(string backUrl)
        {
            ViewBag.BackUrl = backUrl;
            return View();
        }

        [HttpPost]
        public string Login(string name, string password, string backUrl)
        {
            if (CheckAuthroize(name, password)) // 验证用户名密码
            {
                // 用Session标识会话为登录状态
                HttpContext.Session.SetString("user", $"{name}已经登录");
                // 在认证中心保存客户端的登录认证码
                TokenIds.Add(HttpContext.Session.Id, Guid.NewGuid());
            }
            else
            {
                return "/Home/Login";
            }
            return backUrl + "?tokenId=" + TokenIds[HttpContext.Session.Id]; // 生成tokenId发送到客户端
        }

        public bool TokenIdIsValid(string tokenId)
        {
            return TokenIds.ContainsKey(tokenId);
        }

        public IActionResult Verification(string tokenId, string backUrl)
        {
            if (!string.IsNullOrEmpty(tokenId) && TokenIds.ContainsKey(tokenId))
            {
                return Redirect(backUrl);
            }

            return Redirect($"{Request.Scheme}://{Request.Host}/Home/Login?backUrl={backUrl}");
        }
    }
}
