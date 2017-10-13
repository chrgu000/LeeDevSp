using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using TenantApp.Providers;

namespace TenantApp.Controllers
{
    [Route("api/[controller]")]
    public class TestController : Controller
    {
        private PlayListContext _context;

        public TestController(PlayListContext context)
        //public TestController(ITenantProvider tenantProvider)
        {
            _context = context;
            //_context = new PlayListContext(new Microsoft.EntityFrameworkCore.DbContextOptions<PlayListContext>(), tenantProvider);
        }

        public IActionResult GetAll()
        {

            return Json("All");
        }
    }
}
