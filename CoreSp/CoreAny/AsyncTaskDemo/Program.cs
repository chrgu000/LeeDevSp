using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AsyncTaskDemo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            TaskDemo td = new TaskDemo();
            td.GetInfo().WriteLine();
            td.GetInfoAsync().Result.WriteLine();

            Console.Read();
        }
    }

    public class TaskDemo
    {
        public string GetInfo()
        {
            return "Hello,GetInfo().";
        }

        public Task<string> GetInfoAsync()
        {
            return Task.FromResult("Hello,GetInfoAsync().");
        }
    }

    public static class ConsoleExtensions
    {
        public static void WriteLine(this object obj, params string[] arg)
        {
            if (arg == null || arg.Length == 0)
                Console.WriteLine(obj);
            else
                Console.WriteLine(obj.ToString(), arg);
        }
    }
}
