using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Extensions.Logging;

namespace ConfigConnect
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //Console.WriteLine(File.ReadAllText(@"D:\a.b\c_d\e.log"));
            //var builder = new ConfigurationBuilder();
            //builder.SetBasePath(Directory.GetCurrentDirectory());
            //builder.AddJsonFile("appsettings.json");
            //var connectionStringConfig = builder.Build();

            //var config = new ConfigurationBuilder()
            //    .SetBasePath(Directory.GetCurrentDirectory())
            //    .AddJsonFile("appsettings.json")
            //    .AddEntityFrameworkConfig(options =>
            //        { }
            //    );
            LoggerFactory factory = new LoggerFactory();
            var logger = factory.CreateLogger("Catchall Endpoint");
            logger.LogInformation("No endpoint found for request");

            Console.ReadLine();
        }
    }
}
