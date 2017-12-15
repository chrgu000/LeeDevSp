using FD.General.Lib.InputLibrary.Exceptions;
using FD.General.Lib.InputLibrary.Hook;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace FDInput
{
    static class Program
    {
        public static KeyboardHook keyboardHook = new KeyboardHook();

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            new ExceptionHandle(() =>
            {
                keyboardHook.Start();
                Application.Run(new FormMain());
                keyboardHook.Stop();
            }, e =>
            {
                MessageBox.Show(e.Message);
            });
        }
    }
}
