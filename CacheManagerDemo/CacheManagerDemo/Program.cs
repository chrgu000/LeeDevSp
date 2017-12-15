using CacheManager.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CacheManagerDemo
{
    class Program
    {
        static void Main3332(string[] args)
        {
            //Get("http://wx.qlogo.cn/mmopen/V4KHeicA1oWv503MfO0ZIU7dels0l1nsyP9naib4jiarWUVltacC7BTSeRZukMOjJdBUrCSDe4tgJ4pcRAZhjRxC4H2E4kq15Ho/0");
            var str = GetImage("http://wx.qlogo.cn/mmopen/V4KHeicA1oWv503MfO0ZIU7dels0l1nsyP9naib4jiarWUVltacC7BTSeRZukMOjJdBUrCSDe4tgJ4pcRAZhjRxC4H2E4kq15Ho/0");
            var fileName = SaveImage(str);
            Console.WriteLine(fileName);
            
            Console.Read();
        }



        public static string GetImage(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            HttpWebResponse response = (HttpWebResponse)request.GetResponseAsync().Result;
            string responseText = string.Empty;
            using (Stream stream = response.GetResponseStream())
            {
                Bitmap bmp = new Bitmap(stream);
                using (MemoryStream ms = new MemoryStream())
                {
                    bmp.Save(ms, ImageFormat.Jpeg);
                    ms.Position = 0;
                    byte[] buffer = new byte[ms.Length];
                    ms.Read(buffer, 0, buffer.Length);
                    var baseImg = $"data:image/jpg;base64,{Convert.ToBase64String(buffer)}";
                    return baseImg;
                }
            }
        }

        public static string SaveImage(string baseString)
        {
            var regex = new Regex("^data:image\\/([a-z]*);base64,", RegexOptions.Compiled);
            var match = regex.Match(baseString);
            byte[] bytes = Convert.FromBase64String(baseString.Substring(match.Value.Length));
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                Bitmap bmp = new Bitmap(ms);
                var fileName = $"{Guid.NewGuid().ToString().Replace("-", "").ToLower()}.jpg";
                bmp.Save(fileName, ImageFormat.Jpeg);
                return fileName;
            }
        }


        static void Get(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            HttpWebResponse response = (HttpWebResponse)request.GetResponseAsync().Result;
            string responseText = string.Empty;
            using (Stream stream = response.GetResponseStream())
            {
                //StreamReader sr = new StreamReader(stream);
                //char[] buffer = new char[1024];
                //int count;
                //List<byte> buffers = new List<byte>();
                //while ((count = sr.Read(buffer, 0, 1024)) > 0)
                //{
                //    buffers.AddRange(Encoding.Default.GetBytes(buffer));
                //}

                using (MemoryStream ms = new MemoryStream())
                {
                    Bitmap bmp = new Bitmap(stream);
                    bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                    ms.Position = 0;
                    byte[] buffer = new byte[ms.Length];
                    ms.Read(buffer, 0, buffer.Length);
                    var str = Convert.ToBase64String(buffer);
                    Console.WriteLine($"data:image/jpg;base64,{str}");
                    //bmp.Save("abc.jpg");
                }
                //sr.Read(buffer, 0, buffer.Length);
                //var str = Convert.ToBase64String(buffers.ToArray());//(Encoding.UTF8.GetBytes(buffer));
                //Console.WriteLine(str);

                //var buffer2 = Convert.FromBase64String(str);
                //using (MemoryStream ms = new MemoryStream(buffer2))
                //{
                //    Bitmap bmp = new Bitmap(ms);
                //    bmp.Save("abc.jpg");
                //}
            }

        }



        static void Main3(string[] args)
        {
            IList<string> data = new List<string> { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M" };
            int pageIndex = 0, pageSize = 3;
            int pageCount = data.Count / pageSize + (data.Count % pageSize == 0 ? 0 : 1);
            for (pageIndex = 0; pageIndex < pageCount; ++pageIndex)
            {
                var result = data.Skip(pageIndex * pageSize).Take(pageSize).ToList();
                Output($"pageIndex:{pageIndex},pageSize:{pageSize},Count:{result.Count}");
                result.ForEach(r => Output(r));
            }
            Output("End...");

            Console.Read();
        }

        private void TableDeal(IList<string> data, int pageIndex, int pageCount)
        {
            int pageSize = data.Count / pageCount + (data.Count % pageCount == 0 ? 0 : 1);
            var result = data.Skip(pageIndex * pageSize).Take(pageSize).ToList();
            Output($"pageIndex:{pageIndex},pageSize:{pageSize},Count:{result.Count}");
            result.ForEach(r => Output(r));
        }


        static void Main33(string[] args)
        {
            var cache = CacheFactory.Build("getStartedCache", settings =>
            {
                settings.WithSystemRuntimeCacheHandle("handleName");
            });

            cache.Add("keyA", "valueA");
            cache.Put("keyB", 23);
            cache.Update("keyB", v => 42);

            Output($"Key A is {cache.Get("keyA")}");
            Output($"Key B is {cache.Get("keyB")}");
            cache.Remove("keyA");
            Output($"Key A removed? {(cache.Get("keyA") == null)}");

            Console.Read();
        }

        static void Output(string msg)
        {
            Console.WriteLine(msg);
        }
    }
}
