using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Client2.Models;
using Microsoft.AspNetCore.Http;
using System.Net.Http;

namespace Client2.Controllers
{
    public class HomeController : Controller
    {
        private static List<string> Tokens = new List<string>();

        public async Task<IActionResult> Index()
        {
            var tokenId = Request.Query["tokenId"].ToString();
            // 哪tokenId不为空，则为Service 302过来
            if (!string.IsNullOrEmpty(tokenId))
            {
                using (HttpClient client = new HttpClient())
                {
                    // 验证token是否有效
                    var isValid = await client.GetStringAsync($"http://localhost:8000/Home/TokenIdIsValid?tokenId={tokenId}");
                    if (bool.Parse(isValid))
                    {
                        if (!Tokens.Contains(tokenId))
                        {
                            // 记录登录过的Client（主要为了可以统一登出）
                            Tokens.Add(tokenId);
                        }
                        HttpContext.Session.SetString("token", tokenId);
                    }
                }
            }
            // 判断是否登录状态
            string sessToken = HttpContext.Session.GetString("token");
            if (string.IsNullOrEmpty(sessToken) || !Tokens.Contains(sessToken))
            {
                string backUrl = $"{Request.Scheme}://{Request.Host}/Home";
                return Redirect($"http://localhost:8000/Home/Verification?tokenId={sessToken}&backUrl={backUrl}");
            }
            else
            {
                if (!string.IsNullOrEmpty(sessToken))
                    HttpContext.Session.Remove("token");
            }
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
    }
}
