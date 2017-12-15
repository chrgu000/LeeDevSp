using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FD.General.Lib.InputLibrary.Exceptions
{
    /// <summary>
    /// 异常处理
    /// </summary>
    public class ExceptionHandle : IDisposable
    {
        public ExceptionHandle(Action action, Action<Exception> exceptionDeal)
        {
            try
            {
                action?.Invoke();
            }
            catch (Exception ex)
            {
                exceptionDeal?.Invoke(ex);
            }
        }

        public void Dispose()
        {
            
        }
    }
}
