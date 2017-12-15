using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ConsoleDemoEventBus.EventBus
{
    public class EventBus
    {
        private static EventBus _eventBus;
        private static Dictionary<Type, List<Type>> _eventMapping = new Dictionary<Type, List<Type>>(); // 存储key（事件），value（事件处理程序）
        private static object _sync = new object();

        public EventBus()
        {
        }

        public static EventBus Instance
        {
            get
            {
                if (_eventBus == null)
                {
                    lock (_sync)
                    {
                        if (_eventBus == null)
                        {
                            _eventBus = new EventBus();
                            MapEvent2Handler();
                        }
                    }
                }
                return _eventBus;
            }
        }

        /// <summary>
        /// 将事件与事件处理程序映射在一起使用元数据来进行注册
        /// </summary>
        private static void MapEvent2Handler()
        {
            Assembly assembly = Assembly.GetEntryAssembly();
            Type[] types = assembly.GetTypes();

            foreach (Type type in types)
            {
                //Type handlerInterfaceType = type.GetType().GetTypeInfo().GetInterface("IEventHandler`1"); // 事件处理者为value添加到映射中
                //if (handlerInterfaceType != null) // 若是事件处理者，则以其泛型参数为key，事件处理者的集合为value添加到映射中
                //{  Array.Exists(typeof(B).GetInterfaces(), t => t.GetGenericTypeDefinition() == typeof(IA<>))
                //    Type eventType = handlerInterfaceType.GenericTypeArguments[0]; // 只有一个
                var interfaces = type.GetInterfaces();
                if (type.GetTypeInfo().IsGenericType)
                {
                    var tt = type.GetGenericTypeDefinition();
                }
                if (type.GetTypeInfo().IsGenericType && interfaces.Any(t => t.GetGenericTypeDefinition() == typeof(IEventHandler<>)))//.IsAssignableFrom(type))
                {
                    Type eventType = type.GetGenericArguments()[0];
                    if (_eventMapping.ContainsKey(eventType))
                    {
                        List<Type> handlerTypes = _eventMapping[eventType];
                        if (!handlerTypes.Contains(type))
                            handlerTypes.Add(type);
                        _eventMapping[eventType] = handlerTypes;
                    }
                    else
                    {
                        List<Type> handlerTypes = new List<Type> { type };
                        _eventMapping.Add(eventType, handlerTypes);
                    }
                }
            }
        }

        /// <summary>
        /// 发布
        /// 此处没用到队列之类东西，使用直接调用方式
        /// </summary>
        /// <param name="eventData"></param>
        public void Publish(BaseEvent eventData)
        {
            Type eventType = eventData.GetType();
            if (_eventMapping.ContainsKey(eventType))
            {
                foreach (Type item in _eventMapping[eventType])
                {
                    MethodInfo mi = item.GetMethod("Handle");
                    if (mi != null)
                    {
                        object o = Activator.CreateInstance(item);
                        mi.Invoke(o, new object[] { eventData });
                    }
                }
            }
        }
    }
}
