using ConsoleAppEventBus.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleAppEventBus.Events
{
    public class OrderGeneratorEvent : IEvent
    {
        public int OrderID { get; set; }
    }
}
