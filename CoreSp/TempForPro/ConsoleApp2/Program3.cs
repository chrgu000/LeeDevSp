using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp2
{
    class Program3
    {
        static void Main1(string[] args)
        {
            var dd = default(KeyValuePair<int, int>);
           bool sd =  default(KeyValuePair<int, int>).Equals(new KeyValuePair<int,int>(0, 0));

            ClassB classB = new ClassB();
            ClassC classC = new ClassC();
            classB.Test();
            classC.Test();
            ((ClassA)classB).Test();
            ((ClassA)classC).Test();


            Console.Read();
        }
    }

    public class ClassA
    {
        public virtual void Test()
        {
            Console.WriteLine("ClassA::Test");
        }
    }
    public class ClassB : ClassA
    {
        public override void Test()
        {
            Console.WriteLine("ClassB::Test");
        }
    }
    public class ClassC : ClassA
    {
        public new void Test()
        {
            Console.WriteLine("ClassC::Test");
        }
    }

}
