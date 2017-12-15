using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ConsoleApp1
{
    class Program6
    {
        static void Main01(string[] args)
        {
            TemplateMsg templateMsg = new TemplateMsg
            {
                ToUser = "213489120sd",
                MiniProgram = new MiniProgram { AppId = "xiaochengxuappid12345", PagePath = "index?foo=bar" },
                TemplateId = "ngqIpbwh8bUfcSsECmogfXcV14J0tQlEpBO27izEYtY",
                Url = "http://weixin.qq.com/download",
                Data = new List<DataItem>
                   {
                        new DataItem{ Name = "first", Value = "恭喜你购买成功！", Color = "#173177"},
                        new DataItem{ Name = "keynote1", Value = "巧克力！", Color = "#173177"},
                        new DataItem{ Name = "keynote2", Value = "39.8元！", Color = "#173177"},
                        new DataItem{ Name = "keynote3", Value = "2014年9月22日！", Color = "#173177"},
                        new DataItem{ Name = "remark", Value = "欢迎再次购买！！", Color = "#173177"}
                   }
            };

            Console.WriteLine(templateMsg.ToJson());
            templateMsg.MiniProgram = null;
            Console.WriteLine(templateMsg.ToJson());


            Console.Read();
        }
    }
    public class TemplateMsg
    {
        [JsonProperty("touser")]
        public string ToUser { get; set; }

        [JsonProperty("template_id")]
        public string TemplateId { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("miniprogram")]
        public MiniProgram MiniProgram { get; set; }

        [JsonProperty("data")]
        public List<DataItem> Data { get; set; }
    }

    public class MiniProgram
    {
        [JsonProperty("appid")]
        public string AppId { get; set; }

        [JsonProperty("pagepath")]
        public string PagePath { get; set; }
    }

    public class DataItem
    {
        [JsonIgnore]
        public string Name { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("color")]
        public string Color { get; set; }
    }

    static class Extensions
    {
        public static string ToJson(this TemplateMsg templateMsg)
        {
            if (templateMsg == null || templateMsg.Data == null)
                return string.Empty;
            var props = templateMsg.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            IDictionary<string, object> obj = new Dictionary<string, object>();
            var others = props.Where(p => !p.Name.Equals(nameof(templateMsg.Data)));
            IDictionary<string, object> data = new Dictionary<string, object>();
            foreach (var d in templateMsg.Data)
            {
                data.Add(d.Name.ToLower(), d);
            }

            string name;
            object value;
            foreach (var p in others)
            {
                value = p.GetValue(templateMsg);
                if (value == null)
                    continue;
                JsonPropertyAttribute jsa = p.GetCustomAttribute<JsonPropertyAttribute>();
                if (jsa != null)
                {
                    name = jsa.PropertyName;
                }
                else
                {
                    name = p.Name.ToLower();
                }
                obj.Add($"{name}", value);
            }
            obj.Add("data", data);

            return obj.ToJson();
        }

        public static string ToJson(this object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

    }
}
