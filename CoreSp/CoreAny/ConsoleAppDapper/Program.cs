using ConsoleAppDapper.LambdaTmp;
using ConsoleAppDapper.XmlTemp;
using FD.Generic.XmlTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppDapper
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            //BookRe

            //var test = new XmlTest();
            //test.Test();
            //test.Test2();
            //test.Test3();
            //test.Test4();

            var testPerson = new PersonTest();
            testPerson.Test();

            /*
            var lambdaTest = new LambdaTest();
            lambdaTest.CallMethod();
            lambdaTest.CreateMethod();
            lambdaTest.MemberCopy();
            lambdaTest.PropertyEquals();
            lambdaTest.LoopPrint();
            lambdaTest.ExpSqlIn();
            */


            Console.Read();
        }
    }
}
