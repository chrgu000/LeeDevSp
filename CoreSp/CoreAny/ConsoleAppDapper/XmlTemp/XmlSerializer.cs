using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ConsoleAppDapper.XmlTemp
{
    /// <summary>
    /// 请求消息
    /// </summary>
    public class Message
    {
        /// <summary>
        /// 报文内容
        /// </summary>
        public string Content
        {
            get
            {
                string content = "<?xml version='1.0' encoding='GB2312' ?>" + Packet;
                return $"{(6 + content.Length).ToString().PadLeft(6, '0')}{content}";
            }
        }

        public string Packet { get; set; }
    }

    /// <summary>
    /// 响应消息
    /// </summary>
    public class RespMessage
    {
        private string _head, _packet;
        private int _length;
        private MessageSerializer _msgSerializer;

        /// <summary>
        /// 响应成功
        /// </summary>
        public bool Success { get; private set; }
        /// <summary>
        /// 响应长度
        /// </summary>
        public int Length { get; private set; }

        /// <summary>
        /// 根据Success获取请求结果，当Success为true时，则T为Packet，否则T为ErrorMessage
        /// </summary>
        /// <typeparam name="T">获取请求结果对象类型</typeparam>
        /// <returns></returns>
        public T GetResult<T>() where T : class
        {
            object result;
            if (Success)
            {
                result = _msgSerializer.FromXml(_packet);
            }
            else
            {
                result = _msgSerializer.FromErrXml(_packet);
            }
            if (!(result is T))
            {
                var msg = Success ? "成功" : "失败";
                throw new MessageException($"请求{msg}时，结果类型为{result.GetType()}，而指定获取类型为：{typeof(T)}");
            }
            return (T)Convert.ChangeType(result, typeof(T));
        }

        /// <summary>
        /// 响应消息
        /// </summary>
        /// <param name="content">响应的报文内容</param>
        public RespMessage(string content)
        {
            _msgSerializer = new MessageSerializer();
            Resove(content);
        }

        /// <summary>
        /// 解析报文
        /// </summary>
        private void Resove(string content)
        {
            var index = content.IndexOf("<packet>");
            var hIndex = content.IndexOf("</head>");
            if (index == -1 || hIndex == -1 || (index >= hIndex))
            {
                throw new MessageException("响应消息异常");
            }
            string length = content.Substring(0, 6);
            int len;
            if (!int.TryParse(length, out len))
            {
                throw new MessageException("响应消息异常");
            }
            Length = len;
            _packet = content.Substring(index);
            _head = content.Substring(index + 8, hIndex - index - 1);
            Success = XmlTagHelper.GetTagContent(_head, "returnCode", "").Equals("AAAAAAA");
        }
    }

    /// <summary>
    /// 报文
    /// </summary>
    public class Packet
    {
        public Head Head { get; set; }
        public Body Body { get; set; }
    }

    /// <summary>
    /// 报文头
    /// </summary>
    public class Head
    {
        /// <summary>
        /// 交易码
        /// </summary>
        public string TransCode { get; set; }
        /// <summary>
        /// 签名标志
        /// </summary>
        public string SignFlag { get; set; }
        /// <summary>
        /// 企业客户号
        /// </summary>
        public string MasterID { get; set; }
        /// <summary>
        /// 报文号
        /// </summary>
        public string PacketID { get; set; }
        /// <summary>
        /// 交易时间戳
        /// </summary>
        public DateTime TimeStamp { get; set; }
        /// <summary>
        /// 返回代码
        /// </summary>
        public string ReturnCode { get; set; }
    }

    /// <summary>
    /// 请求报文体
    /// </summary>
    public class Body
    {
        public string Signature { get; set; }
    }

    /// <summary>
    /// 异常响应消息
    /// </summary>
    public class ErrorMessage
    {
        /// <summary>
        /// 响应头
        /// </summary>
        public Head Head { get; set; }
        /// <summary>
        /// 错误码
        /// </summary>
        public string ReturnCode { get; set; }
        /// <summary>
        /// 错误信息描述
        /// </summary>
        public string ReturnMsg { get; set; }
    }
    public class MessageException : Exception
    {
        public MessageException() { }

        public MessageException(string msg) : base(msg)
        {
        }

        public MessageException(string msg, Exception innerException) : base(msg, innerException)
        {
        }
    }

    public class MessageSerializer
    {
        public string ToXml(Packet packet)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<packet>");
            var pFields = typeof(Packet).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            if (pFields.Count() > 0)
            {
                VisitFields(sb, packet, pFields);
            }
            sb.Append("</packet>");
            return sb.ToString();
        }

        private void VisitFields(StringBuilder sb, object obj, PropertyInfo[] fields)
        {
            foreach (var field in fields)
            {
                if (field.Name.Equals("ReturnCode"))
                    continue;
                sb.AppendFormat("<{0}>", GetElementName(field.Name));

                if (!field.PropertyType.FullName.StartsWith("System."))
                {
                    object subObj = field.GetValue(obj);
                    var subFields = field.PropertyType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                    if (subFields.Count() > 0)
                    {
                        VisitFields(sb, subObj, subFields);
                    }
                    else
                    {
                        sb.Append(field.GetValue(obj));
                    }
                }
                else
                {
                    sb.Append(field.GetValue(obj));
                }
                sb.AppendFormat("</{0}>", GetElementName(field.Name));
            }
        }

        private string GetElementName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return string.Empty;
            return name.Substring(0, 1).ToLower() + name.Substring(1);
        }

        public ErrorMessage FromErrXml(string xml)
        {
            if (string.IsNullOrEmpty(xml) || !xml.StartsWith("<packet>"))
            {
                return null;
            }
            ErrorMessage errMsg = new ErrorMessage();
            VisitXml(xml, errMsg, typeof(ErrorMessage).GetProperties());
            return errMsg;
        }

        public Packet FromXml(string xml)
        {
            if (string.IsNullOrEmpty(xml) || !xml.StartsWith("<packet>"))
            {
                return null;
            }
            Packet packet = new Packet();
            VisitXml(xml, packet, typeof(Packet).GetProperties());
            return packet;
        }

        private void VisitXml(string xml, object obj, PropertyInfo[] fields)
        {
            foreach (var field in fields)
            {
                Type subType = field.PropertyType;
                if (!subType.FullName.StartsWith("System."))
                {
                    object subObj = Activator.CreateInstance(subType);// field.GetValue(obj);
                    var subFields = subType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                    field.SetValue(obj, subObj);
                    if (subFields.Count() > 0)
                    {
                        VisitXml(xml, subObj, subFields);
                    }
                    else
                    {
                        field.SetValue(subObj, XmlTagHelper.GetTagContent(xml, field.Name.FirstToLower(), ""));
                    }
                }
                else
                {
                    var value = XmlTagHelper.GetTagContent(xml, field.Name.FirstToLower(), "");
                    if (subType == typeof(DateTime))
                    {
                        field.SetValue(obj, DateTime.Parse(value));
                    }
                    else
                    {
                        field.SetValue(obj, value);
                    }
                }
            }
        }
    }

    public static class StringExtensions
    {
        /// <summary>
        /// 首字母小写
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public static string FirstToLower(this string word)
        {
            if (string.IsNullOrEmpty(word))
                return string.Empty;
            return word.Substring(0, 1).ToLower() + word.Substring(1);
        }

        /// <summary>
        /// 首字母大写
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public static string FirstToUpper(this string word)
        {
            if (string.IsNullOrEmpty(word))
                return string.Empty;
            return word.Substring(0, 1).ToUpper() + word.Substring(1);
        }
    }

    public class XmlTagHelper
    {
        /// <summary>  
        /// 获取字符中指定标签的值  
        /// </summary>  
        /// <param name="content">字符串</param>  
        /// <param name="tagName">标签</param>  
        /// <param name="attrib">属性名</param>  
        /// <returns>属性</returns>  
        public static string GetTagContent(string content, string tagName, string attrib)
        {
            string tmpStr = string.IsNullOrEmpty(attrib) ? $"<{tagName}>([\\s\\S]*?)</{tagName}>" :
                $"<{tagName}\\s*{attrib}\\s*=\\s*.*?>([\\s\\S]*?)</{tagName}>";
            Match match = Regex.Match(content, tmpStr, RegexOptions.IgnoreCase);

            string result = match.Groups[1].Value;
            return result;
        }

        /// <summary>  
        /// 获取字符中指定标签的值  
        /// </summary>  
        /// <param name="content">字符串</param>  
        /// <param name="tagName">标签</param>  
        /// <param name="attrib">属性名</param>  
        /// <returns>属性</returns>  
        public static List<string> GetTagContents(string content, string tagName, string attrib)
        {
            string tmpStr = string.IsNullOrEmpty(attrib) ? $"<{tagName}>([\\s\\S]*?)</{tagName}>" :
                $"<{tagName}\\s*{attrib}\\s*=\\s*.*?>([\\s\\S]*?)</{tagName}>";
            MatchCollection matchs = Regex.Matches(content, tmpStr, RegexOptions.IgnoreCase);

            var result = new List<string>();
            foreach (Match match in matchs)
            {
                result.Add(match.Groups[1].Value);
            }
            return result;
        }
    }

    public class XmlTest
    {
        public void Test()
        {
            Message msg = new Message();
            Packet packet = new Packet
            {
                Head = new Head
                {
                    MasterID = "1234567890",
                    PacketID = "12345678901111111112",
                    SignFlag = "1",
                    TransCode = "1112",
                    TimeStamp = DateTime.Now
                },
                Body = new Body
                {
                    Signature = "xxxxxxxxxxxxxdasdffffffffffffffwasdfosadfsadfsdafsadasdf"
                }
            };
            var serializer = new MessageSerializer();
            msg.Packet = serializer.ToXml(packet);
            Console.WriteLine(msg.Packet);
            Console.WriteLine("+++++++++++++++++++++++");
            Console.WriteLine(msg.Content);

            string res = GetTagContent(msg.Packet, "masterID", "");

            Console.WriteLine(res);

            string tmp = " <xml>  <head>    <transCode>1112</transCode>    <signFlag>1</signFlag>   <masterID>1234567890</masterID>  <masterID   id = \"12\">id1234567890</masterID>  <masterID name=\"good\">name1234567890</masterID>    <packetID>12345678901111111112</packetID>    <timeStamp>2017/4/11 14:34:23</timeStamp>  </head>  <body>    <signature>xxxxxxxxxxxxxdasdffffffffffffffwasdfosadfsadfsdafsadasdf</signature>  </body></xml>";
            res = GetTagContent(tmp, "masterID", "id");

            Console.WriteLine(res);
        }

        public void Test2()
        {
            var xml = @"<packet>
<head>
<transCode>4402</transCode>
<signFlag>1</signFlag>
<packetID>1234567890</packetID>     
<timeStamp>2004-07-28 16:14:25</timeStamp>  
<returnCode>AAAAAAA</returnCode> 
</head>
<body>
<signature> 
VQQKEwdFbnRydXN0MS8wLQYDVQQLEyZFbnRydXN0IFBLSSBEZW1vbnN0cmF0aW9uIENlcnRpZmljYXRlczEOMAwGA1UEAxMFQ1JMMjgwHwYDVR0jBBgwFoAUc1Ky8vw9NwyqF99owA46lu1WJbowHQYDVR0OBBYEFG8CunaEVls4u40piTWUgz+1aj51MAkGA1UdEwQCMAAwGQYJKoZIhvZ9B0EABAwwChsEVjQuMAMCA6gwDQYJKoZIhvcNAQEFBQADgYEAXZP2x4EKzMQeefFIW/DkCmwIXvz9RHb3nyna+A4HTaxHxDwZJoB0olIgTjrqcFwba4wiC2mQUapF82KYX5gtJ4XTzZH1HkHs0ZLbI3T5Bxj+bqaPt/2Lq5VEjwwjZ5B4csNML5xb/45Osbt4++Sx4Z7PrVvQlyHTzUC2EZL1h9ExgbwwgbkCAQEwWDBQMQswCQYDVQQGEwJVUzEQMA4GA1UEChMHRW50cnVzdDEvMC0GA1UECxMmRW50cnVzdCBQS0kgRGVtb25zdHJhdGlvbiBDZXJ0aWZpY2F0ZXMCBDuZs54wCQYFKw4DAhoFADANBgkqhkiG9w0BAQEFAARAJ6qQ2n3AXerdThUkyzgtyfLVIg5PV5LsV4ompdp4alVrVKJ9ZdGw+/tsLWMO1c9qmOeDnkshK9cxqTrK3Nys1w
</signature>
<body>
</packet>";

            var serializer = new MessageSerializer();
            var packet = serializer.FromXml(xml);

            Console.WriteLine(packet.Head.PacketID);

            var xml2 = @"<packet>
<head>
<transCode>8924</transCode>
<signFlag>1</signFlag>
<packetID>task010000005542</packetID>
<timeStamp>2010-06-12 23:53:00</timeStamp>
<returnCode>EGG0521</returnCode>
</head>
<body>
<returnCode>EGG0521</returnCode>
<returnMsg>EGG0521:已无符合条件的记录</returnMsg>
</body>
</packet>";
            var errMsg = serializer.FromErrXml(xml2);
            Console.WriteLine(errMsg.ReturnMsg);
        }

        public void Test3()
        {
            var xml3 = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
@"<xml>
  <head>
    <transCode>1112</transCode>
    <signFlag>1</signFlag>
    <masterID>1234567890</masterID>
    <packetID>12345678901111111112</packetID>
    <timeStamp>2017/4/11 16:24:47</timeStamp>
  </head>
  <body>
    <signature>xxxxxxxxxxxxxdasdffffffffffffffwasdfosadfsadfsdafsadasdf</signature>
  </body>
</xml>";
            Console.WriteLine(xml3);
            Console.WriteLine(xml3.Substring(xml3.IndexOf("<xml>")));
        }

        public void Test4()
        {
            var errResp = "324   <?xml version=\"1.0\" encoding=\"gb2312\"?>" +
"<packet><head><transCode>8924</transCode><signFlag>1</signFlag><packetID>task010001000142</packetID><timeStamp>2017/4/13 9:02:06</timeStamp><returnCode>EYY8990</returnCode></head><body><returnCode>EYY8990</returnCode><returnMsg>报文流水号当天必须唯一</returnMsg></body></packet>";
            var respMsg = new RespMessage(errResp);
            if (respMsg.Success)
            {
                var res = respMsg.GetResult<Packet>();
                Console.WriteLine(res.Body.Signature);
            }
            else
            {
                var res = respMsg.GetResult<ErrorMessage>();
                Console.WriteLine($"{res.ReturnCode},{res.ReturnMsg}");
            }
        }

        /// <summary>  
        /// 获取字符中指定标签的值  
        /// </summary>  
        /// <param name="content">字符串</param>  
        /// <param name="tagName">标签</param>  
        /// <param name="attrib">属性名</param>  
        /// <returns>属性</returns>  
        public string GetTagContent(string content, string tagName, string attrib)
        {

            string tmpStr = string.IsNullOrEmpty(attrib) ? $"<{tagName}>(.*?)</{tagName}>" :  //string.Format("<{0}[^>]*?{1}=(['\"\"]?)(?<url>[^'\"\"\\s>]+)\\1[^>]*>", tagName, attrib); //获取<title>之间内容  
                $"<{tagName}\\s*{attrib}\\s*=\\s*.*?>(.*?)</{tagName}>";
            Match match = Regex.Match(content, tmpStr, RegexOptions.IgnoreCase);

            string result = match.Groups[1].Value;
            return result;
        }
    }
}
