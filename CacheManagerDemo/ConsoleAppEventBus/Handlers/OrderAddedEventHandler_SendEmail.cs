using ConsoleAppEventBus.Events;
using ConsoleAppEventBus.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleAppEventBus.Handlers
{
    /// <summary>
    /// 发邮件功能
    /// </summary>
    public class OrderAddedEventHandler_SendEmail : IEventHandler<OrderGeneratorEvent>
    {
        public void Handle(OrderGeneratorEvent evt)
        {
            Console.WriteLine("Order_Number:{0}, Send an email.", evt.OrderID);
        }
    }
}
