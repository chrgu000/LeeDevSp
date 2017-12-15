using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleAppEventBus.Infrastructure
{
    /// <summary>
    /// 事件处理接口
    /// </summary>
    public interface IEventHandler<TEvent> where TEvent : IEvent
    {
        /// <summary>
        /// 处理程序
        /// </summary>
        /// <param name="evt"></param>
        void Handle(TEvent evt);
    }
}
