using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace ConsoleApp1
{
    class Program
    {
        private static readonly string SingleAccountUrl = " http://openapi.xg.qq.com/v2/push/single_account";
        private static readonly string SecretKey = "2a933c95c66dd74b1d61e5e83066bc45";

        static void Main01(string[] args)
        {
            //var url = $"{SingleAccountUrl}?access_id=123432&&zkes=12389&cal_type=0&timestamp=123421389&valid_time=32190231";
            var url = "http://openapi.xg.qq.com/v2/push/single_device?access_id=2100266883&timestamp=1505201419&device_token=c3c6092e878004cc4a2aa79ac02ea0b1a0800676&message_type=1&message={\"content\":\"来自信鸽的测试推送消息\",\"title\":\"测试推送消息\",\"vibrate\":1}";
            Uri uri = new Uri(url);
            String str = $"GET{uri.Host}{uri.AbsolutePath}";

            Console.WriteLine(str);
            Console.WriteLine(GetParameters(url));

            var res = $"{str}{GetParameters(url)}{SecretKey}";
            Console.WriteLine(res);

            var md5 = MD5.Create();
            var dd = BitConverter.ToString(md5.ComputeHash(Encoding.UTF8.GetBytes(res))).Replace("-","").ToLower();
            Console.WriteLine(dd);

            string dds = "GETopenapi.xg.qq.com/v2/push/single_deviceaccess_id=2100266883device_token=c3c6092e878004cc4a2aa79ac02ea0b1a0800676message={\"content\":\"来自信鸽的测试推送消息\",\"title\":\"测试推送消息\",\"vibrate\":1}message_type=1timestamp=15052014192a933c95c66dd74b1d61e5e83066bc45";

            var tt = BitConverter.ToString(md5.ComputeHash(Encoding.UTF8.GetBytes(dds))).Replace("-", "").ToLower();

            Console.WriteLine(tt);

            string ddm = "GETopenapi.xg.qq.com/v2/push/single_deviceaccess_id=2100266883device_token=c3c6092e878004cc4a2aa79ac02ea0b1a0800676message={\"content\":\"来自信鸽的测试推送消息\",\"title\":\"测试推送消息\",\"vibrate\":1}message_type=1timestamp=15052672052a933c95c66dd74b1d61e5e83066bc45";
            tt = BitConverter.ToString(md5.ComputeHash(Encoding.UTF8.GetBytes(ddm))).Replace("-", "").ToLower();
            Console.WriteLine(tt);

            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));

            DateTime dtStart2 = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1), TimeZoneInfo.Local) + TimeZoneInfo.Local.BaseUtcOffset;
            var now = DateTime.Now;
            var ddsss = (now - dtStart).TotalSeconds;
            var ddss2 = (now - dtStart2).TotalSeconds;
            var ls = (int)ddsss;
            Console.WriteLine(ls);

            Console.Read();
        }

        static string GetParameters(string url)
        {
            int index = url.IndexOf('?');
            if (index == -1)
                return string.Empty;
            var strs = url.Substring(index+1).Split('&', StringSplitOptions.RemoveEmptyEntries).OrderBy(s => s.Split('=')[0]);
            return string.Join("", strs);
        }

        static void Main02(string[] args)
        {
            Message message = new Message
            {
                Content = "Where to go",
                RecvUserId = 2,
                SendTime = DateTime.Now,
                SendUserId = 1,
                SendUserName = "Lee",
                SessionId = Guid.NewGuid(),
                Type = 1
            };

           var res = JsonConvert.SerializeObject(message);
            Console.WriteLine(res);

            var props = typeof(Message).GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public).OrderBy(p => p.Name);
            IDictionary<string, object> obj = new Dictionary<string, object>();
            props.ToList().ForEach(p =>
            {
                obj.TryAdd(p.Name, p.GetValue(message));
            });
            var res2 = JsonConvert.SerializeObject(obj);
            Console.WriteLine(res2);

            Console.Read();
        }
    }

    public class Message
    {
        /// <summary>
        /// SessionId
        /// </summary>
        public Guid SessionId { get; set; }

        public long RecvUserId { get; set; }

        public long SendUserId { get; set; }

        /// <summary>
        /// 发送用户
        /// </summary>
        public string SendUserName { get; set; }

        /// <summary>
        /// 消息类型
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 消息正文（非文本消息将压缩后使用base64文本保存）
        /// </summary>
        public string Content { get; set; }

        public DateTime SendTime { get; set; }
    }
}
