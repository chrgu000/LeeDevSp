using FD.General.Lib.InputLibrary.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FD.General.Lib.InputLibrary.Hook
{
    /// <summary>
    /// 键盘钩子
    /// </summary>
    public class KeyboardHook : HookBase, IHook
    {
        private bool _isLocked = true;
        private bool _isStarted = false;

        private const int WH_KEYBOARD_ID = 13; // 线程键盘钩子监听鼠标消息设置为2，全局键盘监听鼠标消息设置为13
        private static int hKeyboardHook = 0; // 声明键盘钩子处理初始值
        
        #region 事件
        /// <summary>
        /// 键松
        /// </summary>
        public event KeyEventHandler KeyUpEvent;
        /// <summary>
        /// 空格
        /// </summary>
        public event Action<int> OnSpaced;
        /// <summary>
        /// 回删除
        /// </summary>
        public event Action OnBacked;
        /// <summary>
        /// 翻页
        /// </summary>
        public event Action<int> OnPaged;
        #endregion

        public void Send(string msg)
        {
            if (!string.IsNullOrEmpty(msg))
            {
                Stop();
                SendKeys.Send("{RIGHT}" + msg);
                Start();
            }
        }

        public void Start()
        {
            // 安装键盘钩子
            if (hKeyboardHook == 0)
            {
                hKeyboardHook = Win32Api.SetWindowsHookEx(WH_KEYBOARD_ID, _hookProc, Win32Api.GetModuleHandle(Process.GetCurrentProcess().MainModule.ModuleName), 0);

                // 如安装失败
                if (hKeyboardHook == 0)
                {
                    Stop();
                    throw new Exception("安装键盘钩子失败");
                }
            }
        }

        public void Stop()
        {
            bool retKeyboard = true;
            if (hKeyboardHook != 0)
            {
                retKeyboard = Win32Api.UnhookWindowsHookEx(hKeyboardHook);
                hKeyboardHook = 0;
            }
            if (!retKeyboard)
                throw new Exception("卸载钩子失败！");
        }


        /// <summary>
        /// 按键处理
        /// </summary>
        /// <param name="nCode"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns>返回1，则结束消息。返回0或调用CallNextHookEx函数则消息出了这个钩子继续往下传递，传递给真正接受者</returns>
        private int KeyboardHookProc(int nCode, int wParam, IntPtr lParam)
        {
            // 按键处理，返回1，则结束消息。返回0或调用CallNextHookEx函数则消息出了这个钩子继续往下传递，传递给真正接受者
            // 侦听键盘事件
            if (nCode >= 0 && wParam == 0x0100)
            {
                KeyboardHookStruct keyboardHookStruct = (KeyboardHookStruct)Marshal.PtrToStructure(lParam, typeof(KeyboardHookStruct));

                // 开关
                if (keyboardHookStruct.vkCode == 20 || keyboardHookStruct.vkCode == 160 || keyboardHookStruct.vkCode == 161)
                {
                    _isLocked = !_isLocked;
                }

                if (_isLocked)
                {
                    #region 翻页
                    if (keyboardHookStruct.vkCode == 33)
                    {
                        OnPaged(-1); // 上一页
                    }
                    else if (keyboardHookStruct.vkCode == 34)
                    {
                        OnPaged(1); // 下一页
                    }
                    #endregion

                    if (_isStarted && keyboardHookStruct.vkCode >= 48 && keyboardHookStruct.vkCode <= 57)
                    {
                        var c = int.Parse(((char)keyboardHookStruct.vkCode).ToString());
                        OnSpaced(c); // 空格
                        _isStarted = false;
                        return 1;
                    }
                    if (_isStarted && keyboardHookStruct.vkCode == 8)
                    {
                        OnBacked(); // 删除
                        return 1;
                    }
                    if ((keyboardHookStruct.vkCode >= 65 && keyboardHookStruct.vkCode <= 90 || keyboardHookStruct.vkCode == 32))
                    {
                        if (keyboardHookStruct.vkCode >= 65 && keyboardHookStruct.vkCode <= 90)
                        {
                            Keys keyData = (Keys)keyboardHookStruct.vkCode;
                            KeyEventArgs e = new KeyEventArgs(keyData);
                            KeyUpEvent(this, e); // 按键
                            _isStarted = true;
                        }
                        else // 32
                        {
                            OnSpaced(0);
                            _isStarted = false;
                        }
                        return 1;
                    }
                    else
                    {
                        return 0; // 后续操作
                    }
                }
            }
            return Win32Api.CallNextHookEx(hKeyboardHook, nCode, wParam, lParam);
        }

        protected override int InvokeHookProc(int nCode, int wParam, IntPtr lpParam)
        {
            return KeyboardHookProc(nCode, wParam, lpParam);
        }
    }
}
