using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json.Linq;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ApiData.Controllers
{
    [Route("[controller]")]
    public class HttpServerController : Controller
    {
        // NETWORK_GET表示发送GET请求
        public const string NETWORK_GET = "NETWORK_GET";
        // NETWORK_POST_KEY_VALUE表示用POST发送键值对数据
        public const string NETWORK_POST_KEY_VALUE = "NETWORK_POST_KEY_VALUE";
        // NETWORK_POST_XML表示用POST发送XML数据
        public const string NETWORK_POST_XML = "NETWORK_POST_XML";
        // NETWORK_POST_JSON表示用POST发送JSON数据
        public const string NETWORK_POST_JSON = "NETWORK_POST_JSON";
        
        // GET api/values/5
        [HttpGet]
        public string Get(string name, string age)
        {
            return $"Receive Msg: name={name}&age={age}";
        }

        // POST api/values
        [HttpPost]
        public IActionResult Post([FromBody]object data)
        {
            StringValues value;
            if (Request.Headers.TryGetValue("action", out value))
            {
                switch(value[0])
                {
                    case NETWORK_POST_KEY_VALUE:
                        return Content("keyvalue:"+ data);
                    case NETWORK_POST_XML:
                        return Content("xml:" + data);
                    case NETWORK_POST_JSON:
                        return Content("json:" + data);
                }
            }
            return Content("");
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
