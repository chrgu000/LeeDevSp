using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiddlewareDemo
{
    public class MessageService : IMessageService
    {
        private readonly GreetingOptions _options;

        public MessageService(GreetingOptions options)
        {
            _options = options;
        }

        public string FormatMessage(string message)
        {
            return $"Good {_options.GreetAt} {_options.GreetTo} - {message}";
        }
    }
}
