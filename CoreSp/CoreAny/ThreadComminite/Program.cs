using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadComminite
{
    public class Program
    {
        public static void Main01(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            WaitHandle[] handles = new WaitHandle[]
            {
                new AutoResetEvent(false),
                new AutoResetEvent(false),
                new AutoResetEvent(false),
                new AutoResetEvent(false),
                new AutoResetEvent(false),
                new AutoResetEvent(false),
                new AutoResetEvent(false),
                new AutoResetEvent(false)
            };
            for (var i = 0; i < handles.Length; ++i)
            {
                ThreadPool.QueueUserWorkItem(ar =>
                {
                    int index = (int)ar;
                    Thread.Sleep(1000);
                    Log($"任务：{index}开始执行！");
                    (handles[index] as AutoResetEvent).Set();
                }, i);
            }

            ThreadPool.QueueUserWorkItem(ar =>
            {
                WaitHandle.WaitAll(handles);
                Log("所有任务都已完成了，不用再等待了~~~");
            });

            Console.Read();
        }

        static void Log(string msg)
        {
            Console.WriteLine($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] {msg}");
        }

        static void Main22(string[] args)
        {
            EventWaitHandle handleA = new AutoResetEvent(false);
            EventWaitHandle handleB = new AutoResetEvent(false);

            //ThreadPool.QueueUserWorkItem(ar=>
            //{
            //    Log("A：我是A，我已经开始运行了！");
            //    Thread.Sleep(1000);
            //    Log("A：我想睡觉了，B你行跑跑吧。");
            //    EventWaitHandle.SignalAndWait()
            //})


            Console.Read();
        }


        static void Main2d2(string[] args)
        {
            var list = new List<int>
            {
                1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16
            };
            var res = list.TakeWhile(i => i > 3);
            res.ToList().ForEach(i =>
            {
                Console.Write($"{i} ");
            });

            //var sequence = (from x in Generate(100, () => randomNumbers.NextDouble() * 100) let y = randomNumbers.NextDouble() * 100 select new { x, y })
            //    .TakeWhile(point => point.x < 75);
            //foreach (var item in sequence) { Console.WriteLine(item); }
            Console.WriteLine();
            Console.Read();
        }

        static void Main(string[] args)
        {
            List<object> list = new List<object>
            {
                "abc",123,3.12,'C'
            };
            var es = list.OfType<char>();
            while (es.Any())
            {
                list.Remove(es.FirstOrDefault());
            }

            // TakeWhile，只有一直满足条件才会返回，否则终止；如第一项不满足，第二项满足，也无数据返回。
            //var res = list.TakeWhile(o => o is string);//{ decimal n; return decimal.TryParse(o.ToString(), out n); }).ToList();
            list.RemoveTypeOf<int>();

            var tws = new List<string>
            {
                "Abc","Bcde","ABCDE","AB","ABC","EFGH","ABCDE"
            };
            var res = tws.TakeWhile(s => s.Length == 3);
            var res2 = tws.SkipWhile(s => s.Length == 3);

            // 第一个不满足则全部跳过
            res.ConsoleWriteLine(); // Abc
            res2.ConsoleWriteLine(); // Bcde ABCDE AB ABC EFGH ABCDE

            Console.Read();
        }
    }

    static class Enumerable
    {
        public static void RemoveTypeOf<T>(this IList<object> enumable)
        {
            var types = enumable.OfType<T>();
            while (types.Any())
            {
                enumable.Remove(types.FirstOrDefault());
            }
        }

        public static void ConsoleWriteLine<T>(this IEnumerable<T> enumarable)
        {
            var enumerator = enumarable.GetEnumerator();
            while (enumerator.MoveNext())
            {
                Console.Write("{0} ", enumerator.Current);
            }
            Console.WriteLine();
        }
    }
}
