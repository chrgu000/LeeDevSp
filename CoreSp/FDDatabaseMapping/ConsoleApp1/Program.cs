using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]
namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            log4net.LogManager.GetLogger("logapp").Info("Hello");

            Console.Read();

            /*
             
            var rep = log4net.LogManager.CreateRepository("fd");
            log4net.Config.BasicConfigurator.Configure(rep);
            log4net.LogManager.GetLogger(rep.Name, "testApp.Logging").Info("XXXXXXXXXXXX");
             */
        }
    }
}
