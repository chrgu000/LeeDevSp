using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AspectDemo.Test_Aspects;

namespace AspectDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            MyService myService = new MyService();
            MyClient myClient = new MyClient(myService);
            myClient.Test();


            Console.Read();
        }
    }
}
