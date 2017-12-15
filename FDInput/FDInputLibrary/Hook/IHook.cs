using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FD.General.Lib.InputLibrary.Hook
{
    /// <summary>
    /// 钩子处理过程
    /// </summary>
    /// <param name="nCode"></param>
    /// <param name="wParam"></param>
    /// <param name="lpParam"></param>
    /// <returns></returns>
    public delegate int HookProc(int nCode, int wParam, IntPtr lpParam);

    /// <summary>
    /// 钩子接口
    /// </summary>
    public interface IHook
    {
        /// <summary>
        /// 安装钩子
        /// </summary>
        void Start();
        /// <summary>
        /// 卸载钩子
        /// </summary>
        void Stop();
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="msg"></param>
        void Send(string msg);
    }
}
