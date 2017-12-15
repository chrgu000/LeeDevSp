using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleDemoEventBus.EventBus
{
    public interface IEventHandler<T> where T : BaseEvent
    {
        void Handle(T eventData);
    }
}
