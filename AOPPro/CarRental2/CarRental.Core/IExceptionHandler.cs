using System;
using System.Collections.Generic;
using System.Text;

namespace CarRental.Core
{
    public interface IExceptionHandler
    {
        void Wrapper(Action action);
    }
}
