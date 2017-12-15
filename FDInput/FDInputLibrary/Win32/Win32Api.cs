using FD.General.Lib.InputLibrary.Hook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FD.General.Lib.InputLibrary.Win32
{
    /// <summary>
    /// Win32 Api函数
    /// </summary>
    public class Win32Api
    {
        #region 消息码
        public const int WM_KEYDOWN = 0x100; // KEYDOWN
        public const int WM_KEYUP = 0x101; // KEYUP
        public const int WM_SYSKEYDOWN = 0x104; // SYSKEYDOWN
        public const int WM_SYSKEYUP = 0x105; // SYSKEYUP
        #endregion

        /// <summary>
        /// 安装一个钩子
        /// </summary>
        /// <param name="idHook"></param>
        /// <param name="lpfn"></param>
        /// <param name="hInstance"></param>
        /// <param name="threadId"></param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, int threadId);

        /// <summary>
        /// 卸载钩子
        /// </summary>
        /// <param name="idHook"></param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern bool UnhookWindowsHookEx(int idHook);

        /// <summary>
        /// 通过信息钩子继续下一个钩子
        /// </summary>
        /// <param name="idHook"></param>
        /// <param name="nCode"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int CallNextHookEx(int idHook,int nCode,int wParam,IntPtr lParam);

        /// <summary>
        /// 取当前线程编号（线程钩子需要用到）
        /// </summary>
        /// <returns></returns>
        [DllImport("kernel32.dll")]
        public static extern int GetCurrentThreadId();

        /// <summary>
        /// 获取当前实例函数，防止钩子失效
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll")]
        public static extern IntPtr GetModuleHandle(string name);

        /// <summary>
        /// 转换指定虚拟键码和键盘状态相应字符或字符串
        /// </summary>
        /// <param name="uVirtKey">[in]指定虚拟键代码进行翻译</param>
        /// <param name="uScanCode">[in]指定硬件扫描码需翻译成英文。高阶设定关键，是（不压）</param>
        /// <param name="lpbKeyState">[in]指定，以256字节数组，包含当前键盘状态。
        /// 每个元素（字节）数组包含状态的一个关键。如高阶位字节是一套，关键为下阶（按下）。
        /// 在低比特，如果设置表明，关键是对切换。在此功能，只有肘位的CAPS LOCK键是相关的。
        /// 在切换状态的NUM个锁和滚动锁定键被忽略。</param>
        /// <param name="lpwTransKey">[out]指针的缓冲区收到翻译字符或字符串</param>
        /// <param name="fuState">[in]指定当前激活菜单。激活为1，否则为0</param>
        /// <returns></returns>
        [DllImport("user32")]
        public static extern int ToAscii(int uVirtKey, int uScanCode, byte[] lpbKeyState, byte[] lpwTransKey, int fuState);

        /// <summary>
        /// 获取键盘状态
        /// </summary>
        /// <param name="vKey"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern int GetKeyboardState(byte[] pbKeyState);

        /// <summary>
        /// 获取键状态
        /// </summary>
        /// <param name="vKey"></param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int GetKeyState(int vKey);

        /// <summary>
        /// 获取异步状态
        /// </summary>
        /// <param name="vKey"></param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int GetAsyncKeyState(int vKey);
    }

    /// <summary>
    /// 键盘结构
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public class KeyboardHookStruct
    {
        public int vkCode; // 定义一个虚拟键码。该代码必须有一个取值范围1至254
        public int scanCode; // 指定硬件扫描码关键
        public int flags; // 键标志
        public int time; // 指定时间戳记这个信息
        public int dwExtraInfo; // 指定额外信息相关信息
    }
}
