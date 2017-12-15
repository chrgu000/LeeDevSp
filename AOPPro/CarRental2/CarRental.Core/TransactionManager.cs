using System;
using System.Collections.Generic;
using System.Text;

namespace CarRental.Core
{
    public class TransactionManager : ITransactionManager
    {
        public void Wrapper(Action action)
        {
            using (var ts = new TransactionScope())
            {

            }
        }
    }
}
