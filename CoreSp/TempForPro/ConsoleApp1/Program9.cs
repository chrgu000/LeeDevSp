using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace ConsoleApp1
{
    class Program9
    {
        static void Main1(string[] args)
        {
            int num = 12;
            int i = 2;

            int num2 = (num - i) >> 1;

            UnionField unionField = new UnionField();
            unionField.Int = 1;

            Output(unionField.Int);
            Output(unionField.Bool);
            Output(unionField.Float);


            Console.Read();
        }

        static void Output(object item)
        {
            Console.WriteLine(item);
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    struct UnionField
    {
        [FieldOffset(0)]
        public int Int;
        [FieldOffset(0)]
        public bool Bool;
        [FieldOffset(0)]
        public float Float;
    }
}
