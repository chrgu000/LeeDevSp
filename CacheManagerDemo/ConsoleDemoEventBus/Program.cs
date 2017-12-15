using ConsoleDemoEventBus.Entities;
using ConsoleDemoEventBus.EventBus;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace ConsoleDemoEventBus
{
    class Program
    {
        static void Main1(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            //Console.OutputEncoding = Encoding.UTF8;
            //StringBuilder sb = new StringBuilder();
            //using (FileStream fs = new FileStream("demo.txt", FileMode.Open, FileAccess.Read))
            //{
            //    BinaryReader br = new BinaryReader(fs, Encoding.UTF8);
            //    var buffer = new byte[1024];
            //    int len = 0;
            //    while ((len = br.Read(buffer, 0, buffer.Length)) > 0)
            //    {
            //        sb.Append(Encoding.UTF8.GetString(buffer, 0, len));
            //    }
            //    br.Dispose();
            //}

            //var content = sb.ToString(); 
            var content = System.IO.File.ReadAllText("demo.txt", Encoding.UTF8);
            var dics = new System.Collections.Generic.Dictionary<char, int>();
            var strs = content.Split(new char[] { '。', '，' }, StringSplitOptions.RemoveEmptyEntries);
            var count = 0;
            foreach (var str in strs)
            {
                var chs = str.Trim().ToCharArray();
                foreach (var c in chs)
                {
                    if (c.Equals((char)65279) || c.Equals(' ')) continue;
                    if (dics.ContainsKey(c))
                    {
                        dics[c]++;
                    }
                    else
                    {
                        dics[c] = 1;
                    }
                    count++;
                }
            }
            var items = dics.Where(d => d.Value > 1);
            if (items.Count() > 0)
            {
                Console.WriteLine("骗人，全篇字数{0}个，一共使用了{1}个汉字，其中重复的字有：", count, dics.Count);
                items.ToList().ForEach(c =>
                {
                    Console.WriteLine("{0}：出现了{1}次", c.Key, c.Value);
                });
            }
            else
            {
                Console.Write("确实没有重复的字，全篇字数{0}个，一共使用了{1}个汉字", count, dics.Count);
            }

            Console.Read();
        }

        static void Main(string[] args)
        {
            EventBus.EventBus bus = EventBus.EventBus.Instance;

            Order order = new Order { OrderId = "20170628ABCDE", OrderDateTime = DateTime.Now, OrderAmount = 1000 };
            bus.Publish(new OrderAddedEvent { Order = order, EventSource = order, EventTime = DateTime.Now });

            Console.Read();
        }
    }
}