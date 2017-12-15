using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiddlewareDemo
{
    public interface IMessageService
    {
        string FormatMessage(string message);
    }
}
