using com.tencent.mars.sample.chat.proto;
using com.tencent.mars.sample.proto;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp4
{
    class Program
    {
        static void Main01(string[] args)
        {
            HelloRequest req = new HelloRequest.Builder()
                .SetUser("JackieLee")
                .SetText("Hello")
                .Build();

            byte[] buffer = req.ToByteArray();

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:8080/");
            ByteArrayContent content = new ByteArrayContent(buffer);
            content.Headers.Add("Content-Type", "application/octet-stream");
            Stream respStream = client.PostAsync("mars/hello", content).Result.Content.ReadAsStreamAsync().Result;
            HelloResponse resp = HelloResponse.ParseFrom(respStream);

        }

        static void Main02(string[] args)
        {
            ConversationListRequest req = new ConversationListRequest.Builder()
                .SetAccessToken("")
                .SetType((int)ConversationListRequest.Types.FilterType.DEFAULT)
                .Build();

            byte[] buffer = req.ToByteArray();

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:8080/");
            ByteArrayContent content = new ByteArrayContent(buffer);
            content.Headers.Add("Content-Type", "application/octet-stream");
            Stream respStream = client.PostAsync("mars/getconvlist", content).Result.Content.ReadAsStreamAsync().Result;
            ConversationListResponse resp = ConversationListResponse.ParseFrom(respStream);
            foreach (var c in resp.ListList)
            {
                "get conversation: name={0}, topic={1}, notice={2}".Log(c.Name, c.Topic, c.Notice);
            }

            /*
                get conversation: name=Mars, topic=0, notice=STN Discuss
                get conversation: name=Mars, topic=1, notice=Xlog Discuss
                get conversation: name=Mars, topic=2, notice=SDT Discuss
             */
            Console.Read();
        }

        public static short MAGIC = 0x0110;
        public static short PRODUCT_ID = 200;
        public static String LONG_LINK_HOST = "localhost";
        public static int[] LONG_LINK_PORTS = new int[] { 8081 };
        public static int SHORT_LINK_PORT = 8080;

        static void Main(string[] args)
        {
           SendMessageRequest req = new SendMessageRequest.Builder()
                .SetAccessToken("test_token")
                .SetFrom("jackie")
                .SetTo("good")
                .SetText("Hello every one,I'm Jackie Lee.")
                .SetTopic("STN Discuss")
                .Build();

            byte[] buffer = req.ToByteArray();

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:8080/");
            ByteArrayContent content = new ByteArrayContent(buffer);
            content.Headers.Add("Content-Type", "application/octet-stream");
            Stream respStream = client.PostAsync("mars/sendmessage", content).Result.Content.ReadAsStreamAsync().Result;
            SendMessageResponse resp = SendMessageResponse.ParseFrom(respStream);
            
            

            Console.Read();
        }
    }

    public static class ObjExtension
    {
        /// <summary>
        /// 日志
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void Log(this object obj, params object[] args)
        {
            if (args?.Length > 0)
                Console.WriteLine(obj.ToString(), args);
            else
                Console.WriteLine(obj);
        }
    }

}
