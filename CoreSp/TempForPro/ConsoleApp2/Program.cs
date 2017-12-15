using System;
using System.Text.RegularExpressions;

namespace ConsoleApp2
{
    class Program
    {
        static void Main1(string[] args)
        {
            object o = true;
            var de = o.GetBool();



            Console.Read();
        }


    }

    static class Extensions
    {
        static Regex boolRegex = new Regex("(?<info>(true|false))", RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public static bool? GetBool(this object o)
        {
            if (!bool.TryParse(boolRegex.Match(o.ToString(string.Empty)).Groups["info"].Value, out bool info))
            {
                return null;
            }
            return info;
        }

        public static string ToString(this object obj, string defValue)
        {
            if (obj == null)
            {
                return defValue;
            }
            return obj.ToString();
        }
    }
}
