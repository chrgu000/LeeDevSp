using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace WebCache
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                //.UseStartup<StartupA>() // MemoryCache
                //.UseStartup<StartupB>() // Redis
                //.UseStartup<StartupC>() // SqlServer
                .UseStartup<StartupD>() //  整个HTTP响应缓存
                .UseApplicationInsights()
                .Build();

            host.Run();
        }
    }
}
