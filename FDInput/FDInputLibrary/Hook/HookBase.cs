using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FD.General.Lib.InputLibrary.Hook
{
    /// <summary>
    /// 钩子基类
    /// </summary>
    public abstract class HookBase
    {
        protected HookProc _hookProc;
        public HookBase()
        {
            _hookProc = InvokeHookProc;
        }

        protected abstract int InvokeHookProc(int nCode, int wParam, IntPtr lpParam);
    }
}
