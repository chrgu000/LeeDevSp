using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ConsoleRosyln
{
    public class Program
    {
        public static void Main01(string[] args)
        {

            var method = typeof(ITest).GetMethods().Where(e => e.IsGenericMethod && e.GetGenericArguments().Length == 1 && e.GetParameters().Length == 2).FirstOrDefault();
            var s = method.GetParameters();
            var d = method.GetGenericArguments();

            var res =  method.MakeGenericMethod(typeof(long)).Invoke(new Test(), new object[] { 100, "well" });
            Console.WriteLine(res);

            Console.Read();
        }
    }
        
    public interface ITest
    {
        string Get<T>(T a, string b);
        string Get2(string a);
    }

    public class Test : ITest
    {
        public string Get<T>(T a, string b)
        {
            return $"{a} - {b}";
        }

        public string Get2(string a)
        {
            return "Get2";
        }
    }


    class FDCompiler
    {
        public static void Compile(string content)
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(content);
            List<MetadataReference> references = new List<MetadataReference>
            {
                //MetadataReference.CreateFromFile(typeof(object).Assembly.Location)
            };
        }
    }
}
