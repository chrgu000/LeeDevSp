using ConsoleAppEventBus.Events;
using ConsoleAppEventBus.Handlers;
using ConsoleAppEventBus.Infrastructure;
using System;
using System.Text;

namespace ConsoleAppEventBus
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            EventBus.Instance.Subscribe(new OrderAddedEventHandler_SendEmail());
            var entity = new OrderGeneratorEvent { OrderID = 1 };
            Console.WriteLine("生成一个订单，单号为：{0}", entity.OrderID);
            EventBus.Instance.Publish(entity);

            Console.Read();
        }
    }
}