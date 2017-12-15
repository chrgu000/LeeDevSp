using System;
using System.IO;
using System.Linq;
using System.Threading;
/*
 测试结论：.net core加载c++类库，只能加载平台为x64的，而不能加载x86的
 XL中类库为x86的，故会加载失败
*/
namespace XLDownload
{
    class Program
    {
        private const string KEY_URL = "url";
        private const string KEY_SAVETO = "saveto";
        private const string KEY_EXIT = "exit";

        static Action<string> Output = str => Console.WriteLine(str);
        static Action ShowMenu = () =>
         {
             Output("******************************************");
             Output("参数：");
             Output("url:下载的文件url路径");
             Output("saveTo:下载文件保存路径（默认为当前目录下的：files目录）");
             Output("示例：");
             Output("url=http://baidu.com/xxxx.jpg saveTo=D:/download 即可将Url指向的jpg文件下载到D盘中download目录");
             Output("******************************************");
             Output("请输入参数（输入exit退出程序）：");
         };

        static void Main2(string[] args)
        {
            Menu:
            ShowMenu();
            var input = Console.ReadLine();
            if (!Resove(input, out Parameter parameter))
            {
                if (!parameter.IsExit)
                    goto Menu;
                Output("Byte!");
                return;
            }

            if (parameter.SaveTo.IsNullOrEmpty())
            {
                parameter.SaveTo = "files";
            }

            if (!Directory.Exists(parameter.SaveTo))
            {
                Directory.CreateDirectory(parameter.SaveTo);
            }

            XLDownloader downloader = new XLDownloader(parameter, Output);

            Output("是否继续下载操作？(y/n)");
            var q = Console.ReadLine();
            if (q.IgnoreCaseEquals("y") || q.IgnoreCaseEquals("yes"))
            {
                goto Menu;
            }

            Output("Byte!");
            Console.Read();
        }

        static bool Resove(string args, out Parameter parameter)
        {
            parameter = new Parameter();
            if (string.IsNullOrWhiteSpace(args))
            {
                Output("参数不参为空");
                return false;
            }

            var keys = args.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            foreach (var k in keys)
            {
                if (k.IgnoreCaseEquals(KEY_EXIT))
                {
                    parameter.IsExit = true;
                    return false;
                }
                var items = k.Split('=', StringSplitOptions.RemoveEmptyEntries);
                if (items.Length != 2)
                {
                    Output($"参数{k}输入有误");
                    return false;
                }
                if (items[0].IgnoreCaseEquals(KEY_URL))
                {
                    parameter.Url = items[1];
                }
                else if (items[0].IgnoreCaseEquals(KEY_SAVETO))
                {
                    parameter.SaveTo = items[1];
                }
            }
            return parameter.Url.IsValidUrl();
        }
    }

    class XLDownloader
    {
        private IntPtr ptrDownloadTask;
        private Timer _timer;
        private System.Timers.Timer timer;

        private Action<string> Trace { get; }

        public bool Success { get; private set; }
        public bool Finished { get; private set; }

        public XLDownloader(Parameter parameter, Action<string> trace)
        {
            Trace = trace;

            timer = new System.Timers.Timer();
            timer.Interval = 500;
            timer.Elapsed += timer1_Tick;

            Download(parameter);
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            XL.DownTaskInfo taskInfo = new XL.DownTaskInfo();
            var qq = XL.XL_QueryTaskInfoEx(ptrDownloadTask, taskInfo);
            Trace?.Invoke("下载进度：" + (int)(taskInfo.fPercent * 100) + "%");

            if (taskInfo.stat == XL.DOWN_TASK_STATUS.TSC_COMPLETE)
            {
                Trace?.Invoke("下载进度：" + "下载成功！");
                timer.Enabled = false;
            }
        }

        private void Download(Parameter parameter)
        {
            try
            {
                if (!XL.XL_Init())
                {
                    Trace?.Invoke("XL_Init初始化失败");
                    return;
                }
                XL.DownTaskParam param = new XL.DownTaskParam
                {
                    IsResume = 0,
                    //szTaskUrl = parameter.Url,
                    szTaskUrl = "http://fs.vip.pc.kugou.com/201710131333/37a71efa9b14f17114a2d517d8f85188/G075/M07/0A/03/iw0DAFffseSARTOUASifkQnHJbE89.flac",
                    szFilename = "邝美云 - 去.flac",
                    szSavePath = parameter.SaveTo
                };
                ptrDownloadTask = XL.XL_CreateTask(param);

                //XL.DownTaskInfo taskInfo = new XL.DownTaskInfo();
                //_timer = new Timer(new TimerCallback(state =>
                //{
                //    var qtInfo = XL.XL_QueryTaskInfoEx(ptrDownloadTask, taskInfo);
                //    Trace?.Invoke($"下载进度：{(int)(taskInfo.fPercent * 100)}%，速度：{(taskInfo.nSpeed / 1024.0 / 1024.0).ToString("F2")}MB/s");
                //    if (taskInfo.stat == XL.DOWN_TASK_STATUS.TSC_COMPLETE)
                //    {
                //        Trace?.Invoke("下载进度：下载成功！");
                //        _timer.Dispose();
                //        _timer = null;
                //        Success = true;
                //        Finished = true;
                //    }
                //}), null, 0, 500);

                var status = XL.XL_StartTask(ptrDownloadTask);
            }
            catch (Exception e)
            {
                Trace?.Invoke(e.Message);
                Success = false;
                Finished = true;
            }
        }
    }

    class Parameter
    {
        public string Url { get; set; }
        public string SaveTo { get; set; }
        public bool IsExit { get; set; }
    }

    static class Extensions
    {
        public static bool IgnoreCaseEquals(this string str1, string str2)
        {
            return str1.Equals(str2, StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        public static bool IsValidUrl(this string url)
        {
            return !IsNullOrEmpty(url) && url.IndexOf("://") > 0;
        }
    }
}
