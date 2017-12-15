using System;
using System.Collections.Generic;
using System.Text;

namespace CarRental.Core
{
    public interface ITransactionManager
    {
        void Wrapper(Action action);
    }
}
