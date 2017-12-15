using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace DefineWebApiRule
{
    [Route("test")]
    public class TestService : ITestService
    {
        [Route("{name}"), HttpGet]
        public string Test(string name)
        {
            return $"Hello {name}";
        }
    }
}
