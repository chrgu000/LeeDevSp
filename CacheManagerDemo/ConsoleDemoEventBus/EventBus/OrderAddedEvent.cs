using ConsoleDemoEventBus.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleDemoEventBus.EventBus
{
    public class OrderAddedEvent : BaseEvent
    {
        public Order Order { get; set; }
    }
}
