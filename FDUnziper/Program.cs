using System;
using System.IO;

namespace FDUnziper
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine("Hello World!");
            Menu:
            ShowMenu();
            var line = Console.ReadLine();
            var argInfos = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (argInfos.Length < 2)
            {
                ShowMenu();
                goto Menu;
            }
            if (Resove(argInfos, out string exePath, out string path, out string target))
            {
                if (string.IsNullOrWhiteSpace(exePath))
                {
                    exePath = "C:\\Program Files\\WinRAR\\winrar.exe";
                }
                if (!File.Exists(exePath))
                {
                    Output($"winrar：{exePath}不存在，请输入正确路径");
                    ShowMenu();
                    goto Menu;
                }
                FDHelper.Instance.Initialize(exePath, target);
                FDHelper.Instance.Unzip(path);
                Output("解压完毕！");
            }
            else
            {
                Output("输入参数错误");
                ShowMenu();
                goto Menu;
            }
        }

        static bool Resove(string[] args, out string exePath, out string path, out string target)
        {
            exePath = path = target = string.Empty;
            int len = 0;
            foreach (string arg in args)
            {
                var items = arg.Split('=', StringSplitOptions.RemoveEmptyEntries);
                if (items == null || items.Length != 2)
                {
                    Output($"参数项目：{arg}输入非法");
                    return false;
                }
                if (items[0].Equals("exePath", StringComparison.OrdinalIgnoreCase))
                {
                    exePath = items[1];
                    len++;
                }
                if (items[0].Equals("path", StringComparison.OrdinalIgnoreCase))
                {
                    path = items[1];
                    len++;
                }
                if (items[0].Equals("targetPath", StringComparison.OrdinalIgnoreCase))
                {
                    target = items[1];
                    len++;
                }
            }
            if (len < 2)
            {
                ShowMenu();
                return false;
            }
            return true;
        }

        static void ShowMenu()
        {
            Output("=======================================");
            Output("exePath:winrar所在路径，默认为：C:\\Program Files\\WinRAR\\WinRar.exe");
            Output("path:输入为rar所在目录");
            Output("targetPath:输入为rar解压后存放的目录");
            Output("如：path=d:\\rarfile targetPath=d:\\result ");
            Output("=======================================");
        }

        static void Output(string msg)
        {
            Console.WriteLine(msg);
        }
    }
}
