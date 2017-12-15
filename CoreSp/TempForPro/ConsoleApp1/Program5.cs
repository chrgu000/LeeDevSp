using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ConsoleApp1.Sub
{
    class Program5
    {
        static void Main(string[] args)
        {
            List<string> toUsers = new List<string> { "abce", "3214239", "namesaa" };
            TextMsg textMsg = new TextMsg { ToUsers = toUsers, Content = "Hello, nice to meet you!" };
            NormalMediaMessage voice = new NormalMediaMessage(MessageType.Voice) { MediaId = "adsafasdfdsafd", ToUsers = toUsers };
            NormalMediaMessage image = new NormalMediaMessage(MessageType.Image) { MediaId = "abc312431241", ToUsers = toUsers };
            VideoMessage videoMessage = new VideoMessage { ToUsers = toUsers, MediaId = "ab32143214", Title = "好看视频", Description = "这真是好看的视频" };
            CardMessage cardMessage = new CardMessage { ToUsers = toUsers, CardId = "213431298912341234213" };

            NormalMediaMessage image2 = new NormalMediaMessage(MessageType.Image) { MediaId = "abc312431241", ToUser = "dwo123489" };

            Output(textMsg.ToJson());
            Output(voice.ToJson());
            Output(image.ToJson());
            Output(videoMessage.ToJson());
            Output(cardMessage.ToJson());
            Output(image2.ToJson());

            Console.Read();
        }

        static void Output(string msg)
        {
            Console.WriteLine(msg);
        }
    }

    public enum MessageType : byte
    {
        /// <summary>
        /// 图文消息
        /// </summary>
        MpNews = 0,
        /// <summary>
        /// 文本
        /// </summary>
        Text,
        /// <summary>
        /// 语音
        /// </summary>
        Voice,
        /// <summary>
        /// 图片
        /// </summary>
        Image,
        /// <summary>
        /// 视频
        /// </summary>
        MpVideo,
        /// <summary>
        /// 卡券
        /// </summary>
        WxCard
    }

    public abstract class Message
    {
        /// <summary>
        /// 单个的touser，单个优先
        /// </summary>
        [JsonProperty("touser")]
        public string ToUser { get; set; }

        /// <summary>
        /// 批量的touser
        /// </summary>
        [JsonProperty("touser")]
        public List<string> ToUsers { get; set; }

        [JsonProperty("msgtype")]
        public MessageType MessageType { get; protected set; }
    }

    /// <summary>
    /// 图文消息
    /// </summary>
    public class MpNews : Message
    {
        [JsonProperty("media_id")]
        public string MediaId { get; set; }

        [JsonProperty("send_ignore_reprint")]
        public int SendIgnoreReprint { get; set; }

        public MpNews()
        {
            MessageType = MessageType.MpNews;
        }
    }

    /// <summary>
    /// 文本消息
    /// </summary>
    public class TextMsg : Message
    {
        [JsonProperty("content")]
        public string Content { get; set; }
        public TextMsg()
        {
            MessageType = MessageType.Text;
        }
    }

    /// <summary>
    /// 语音、图片
    /// </summary>
    public class NormalMediaMessage : Message
    {
        [JsonProperty("media_id")]
        public string MediaId { get; set; }

        public NormalMediaMessage(MessageType messageType)
        {
            MessageType = messageType;
        }
    }

    /// <summary>
    /// 视频
    /// </summary>
    public class VideoMessage : NormalMediaMessage
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        public VideoMessage() : base(MessageType.MpVideo)
        {
        }
    }

    /// <summary>
    /// 卡券
    /// </summary>
    public class CardMessage : Message
    {
        [JsonProperty("card_id")]
        public string CardId { get; set; }

        public CardMessage()
        {
            MessageType = MessageType.WxCard;
        }
    }

    public static class Extensions
    {
        public static string ToJson(this Message message)
        {
            if (message == null)
                return string.Empty;
            var props = message.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            IDictionary<string, object> obj = new Dictionary<string, object>();
            var toUser = props.FirstOrDefault(p => p.Name.Equals(nameof(message.ToUser)));
            var toUsers = props.FirstOrDefault(p => p.Name.Equals(nameof(message.ToUsers)));
            var others = props.Where(p => p != toUser && p != toUsers && !p.Name.Equals(nameof(message.MessageType)));
            IDictionary<string, object> other = new Dictionary<string, object>();
            if (toUser.GetValue(message) != null)
            {
                obj.Add("touser", toUser.GetValue(message));
            }
            else
            {
                obj.Add("touser", toUsers.GetValue(message));
            }
            var msgType = message.MessageType.ToString().ToLower();
            string name;
            foreach (var pro in others)
            {
                JsonPropertyAttribute jsa = pro.GetCustomAttribute<JsonPropertyAttribute>();
                if (jsa != null)
                {
                    name = jsa.PropertyName;
                }
                else
                {
                    name = pro.Name.ToLower();
                }
                other.Add($"{name}", pro.GetValue(message));
            }
            obj.Add(msgType, other);
            obj.Add("msgtype", msgType);
            return JsonConvert.SerializeObject(obj);
        }
    }
}
