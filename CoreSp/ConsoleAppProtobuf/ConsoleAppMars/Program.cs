using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;

namespace ConsoleAppMars
{
    class Program
    {
        /*
         public static final String OPTIONS_HOST = "host";
    public static final String OPTIONS_CGI_PATH = "cgi_path";
    public static final String OPTIONS_CMD_ID = "cmd_id";
    public static final String OPTIONS_CHANNEL_SHORT_SUPPORT = "short_support";
    public static final String OPTIONS_CHANNEL_LONG_SUPPORT = "long_support";
    public static final String OPTIONS_TASK_ID = "task_id";
             */
        static void Main1(string[] args)
        {
            WebRequest req = WebRequest.Create("http://192.168.1.119:8080/mars/getconvlist");
            WebResponse resp = req.GetResponseAsync().Result;
            BinaryReader br = new BinaryReader(resp.GetResponseStream());
            br.ReadString();


            Console.WriteLine("Hello World!");
        }

        static void Main02(string[] args)
        {
            Connect();
            Console.Read();
        }

        static async void Connect()
        {
            TcpClient client = new TcpClient(AddressFamily.InterNetwork);
            await client.ConnectAsync(new IPAddress(new byte[] { 192, 168, 1, 1 }), 8081);
            var writer = new BinaryWriter(client.GetStream());
            var reader = new BinaryReader(client.GetStream());

            int FIXED_HEADER_SKIP = 4 + 4 + 4 + 4 + 4;
            //public int headLength;
            //public int clientVersion;
            //public int cmdId;
            //public int seq;
            writer.Write(FIXED_HEADER_SKIP);
            writer.Write(200);
            writer.Write(6);
            writer.Write(1);
            writer.Write(0);
            writer.Flush();

            var str = reader.ReadString();
            Console.WriteLine(str);
        }
    }
}