using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleDemoEventBus.EventBus
{
    public class OrderAddedEventHandler : IEventHandler<OrderAddedEvent>
    {
        public void Handle(OrderAddedEvent eventData)
        {
            Output("\r\n");
            Output("订单的信息：");
            Output("\t订单号：", eventData.Order.OrderId);
            Output("\t订单金额：", eventData.Order.OrderAmount);
            Output("\t下单时间：", eventData.Order.OrderDateTime);
        }

        private void Output(string msg, params object[] parameters)
        {
            Console.WriteLine(msg, parameters);
        }
    }
}
