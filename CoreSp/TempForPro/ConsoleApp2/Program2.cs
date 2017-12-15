using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ConsoleApp2
{
    class Program2
    {
        static void Main1(string[] args)
        {
            using (new SimpleDisposable(() =>
            {
                Console.WriteLine("End...");
            }))
            {
                Console.WriteLine("This is the normal logic dealing...");
            }
            Console.WriteLine("Byte!");

            Console.Read();
        }
    }

    public class SimpleDisposable : IDisposable
    {
        /// <summary>
        /// The function that is called when dispose<br/>
        /// 在销毁时调用的函数<br/>
        /// </summary>
        protected Action OnDispose { get; set; }
        /// <summary>
        /// Is method executed<br/>
        /// 函数是否已执行<br/>
        /// </summary>
        protected int Disposed = 0;

        /// <summary>
        /// Initialized<br/>
        /// 初始化<br/>
        /// </summary>
        public SimpleDisposable(Action onDispose)
        {
            OnDispose = onDispose;
        }

        /// <summary>
        /// Call the method if it's not called before<br/>
        /// 调用函数, 如果函数之前未被执行<br/>
        /// </summary>
        public void Dispose()
        {
            if (Interlocked.Exchange(ref Disposed, 1) == 0)
            {
                OnDispose();
            }
        }

        /// <summary>
        /// Finalizer<br/>
        /// 析构函数<br/>
        /// </summary>
        ~SimpleDisposable()
        {
            Dispose();
        }
    }
}
