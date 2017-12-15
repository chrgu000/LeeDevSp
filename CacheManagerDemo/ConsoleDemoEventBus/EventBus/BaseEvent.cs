using System;

namespace ConsoleDemoEventBus.EventBus
{
    public class BaseEvent
    {
        /// <summary>
        /// 事件发生时间
        /// </summary>
        public DateTime EventTime { get; set; }

        /// <summary>
        /// 事件源
        /// </summary>
        public object EventSource { get; set; }

    }
}