using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace DownloadDynamicPictureAssist
{
    class ImageDownloader
    {
        private const int maxRepeatTimes = 5;
        private readonly string _url;
        private readonly string _savePath;
        private readonly int _count;
        private bool _toStop;
        private int _repeatTimes;
        private readonly Random _rand;

        public ImageDownloader(string url, string savePath, int count)
        {
            if (!Directory.Exists(savePath))
            {
                try
                {
                    Directory.CreateDirectory(savePath);
                }
                catch
                {
                    throw new FileNotFoundException($"指定保存路径：{savePath}不存在");
                }
            }
            _url = url;
            _savePath = savePath;
            _count = count;
            _repeatTimes = 0;
            _rand = new Random(DateTime.Now.Millisecond);
        }

        private void TryTimeOut(Action action, int times = 3)
        {
            try
            {
                action();
            }
            catch
            {
                times--;
                if (times > 0)
                {
                    TryTimeOut(action, times);
                }
            }
        }

        private string GetFileName()
        {
            var code = "abcdefghijklmnopqrstuvwxyz0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var codes = code.ToArray();
            StringBuilder sb = new StringBuilder();
            for (var i = 0; i < 15; ++i)
            {
                sb.AppendFormat("{0}", codes[_rand.Next(code.Length)]);
            }
            string fileName = sb.ToString();
            fileName = Path.Combine(_savePath, fileName);
            if (!File.Exists(fileName))
                return fileName;
            return GetFileName();
        }

        private string DownloadImage()
        {
            HttpClient client = new HttpClient();

            HttpResponseMessage response = null;
            TryTimeOut(() =>
            {
                response = client.GetAsync(_url).Result;
            });

            if (response == null)
            {
                Output("请求超时，下载失败！");
                _toStop = true;
                return string.Empty;
            }
            var fileName = "";
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var segments = response.RequestMessage.RequestUri.Segments;
                fileName = segments[segments.Length - 1];
                if (string.IsNullOrEmpty(fileName))
                {
                    fileName = GetFileName();
                }
                fileName = Path.Combine(_savePath, fileName);
                if (File.Exists(fileName))
                {
                    Output($"下载图片{fileName}已存在，目前总重复下载操作{_repeatTimes}次");
                    _repeatTimes++;
                    if (_repeatTimes >= maxRepeatTimes)
                    {
                        _toStop = true;
                        return fileName;
                    }
                }
                else
                {
                    _repeatTimes = 0;
                }
                using (FileStream fs = new FileStream(fileName, FileMode.Create))
                {
                    byte[] buffer = response.Content.ReadAsByteArrayAsync().Result;
                    fs.Write(buffer, 0, buffer.Length);
                }
            }
            return fileName;
        }

        public void Download()
        {
            string fileName;
            int count = 0;
            if (_count == 1)
            {
                fileName = DownloadImage();
                DownloadTip(fileName);
                count = 1;
            }
            else if (_count == -1)
            {
                while (!_toStop)
                {
                    fileName = DownloadImage();
                    DownloadTip(fileName);
                    count++;
                }
            }
            else
            {
                for (int i = 0; i < _count; ++i)
                {
                    fileName = DownloadImage();
                    DownloadTip(fileName);
                    count++;
                }
            }
            Output($"本次一共{count}张图片，下载完毕!");
            Console.WriteLine();
        }

        private void Output(string msg)
        {
            Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {msg}");
        }

        private void DownloadTip(string fileName)
        {
            Console.WriteLine($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] 下载\"{fileName}\"完毕");
        }
    }
}
