using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

/*
 测试结论：.net core加载c++类库，只能加载平台为x64的，而不能加载x86的     
*/

namespace XLDownload
{
    class Program2
    {
        static void Main(string[] args)
        {
            int res = SimpleCalc.Sum(2, 10);
            Console.WriteLine(res);
            res = SimpleCalc.Minus(16, 10);
            Console.WriteLine(res);

            Console.Read();
        }
    }

    static class SimpleCalc
    {
        [DllImport("SimipleCalcLib.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static int Sum(int x, int y);

        [DllImport("SimipleCalcLib.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static int Minus(int x, int y);
    }
}
