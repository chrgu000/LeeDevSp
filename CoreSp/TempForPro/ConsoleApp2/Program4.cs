using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp2
{
    class Program4
    {
        static void Main1(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            string hello = "Hello";
            int g1 = Encoding.GetEncoding("GBK").GetByteCount(hello);
            int u1 = Encoding.Unicode.GetByteCount(hello);
            int u2 = Encoding.UTF8.GetByteCount(hello);

            string hello2 = "你好";
            int g2 = Encoding.GetEncoding("GBK").GetByteCount(hello2);
            int u3 = Encoding.Unicode.GetByteCount(hello2);
            int u4 = Encoding.UTF8.GetByteCount(hello2);

            Console.WriteLine($"{g1},{u1},{u2}"); // 5,10,5
            Console.WriteLine($"{g2},{u3},{u4}"); // 4,4,6

            Console.Read();
        }
    }
}
