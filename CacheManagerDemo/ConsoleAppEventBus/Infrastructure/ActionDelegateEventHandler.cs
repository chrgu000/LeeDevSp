using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleAppEventBus.Infrastructure
{
    public class ActionDelegateEventHandler<TEvent> : IEventHandler<TEvent> where TEvent : class, IEvent
    {
        public ActionDelegateEventHandler(Action<TEvent> eventHandlerFunc)
        {
            
        }

        public void Handle(TEvent evt)
        {
            
        }
    }
}
