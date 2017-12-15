using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1
{
    class Program8
    {
        static void Main1(string[] args)
        {
            ChildBaseC c = new ChildBaseC();
            c.Do();


            Console.Read();
        }
    }

    abstract class BaseC
    {
        public event Action<object, EventArgs> OkEvent;
    }

    class ChildBaseC : BaseC
    {
        public ChildBaseC()
        {
            OkEvent += ChildBaseC_OkEvent;
        }

        public void Do()
        {
            ChildBaseC_OkEvent(this, new EventArgs());
        }

        private void ChildBaseC_OkEvent(object arg1, EventArgs arg2)
        {
            Console.WriteLine("&&&&&&&&&&&&&&&&");
        }
    }
}
