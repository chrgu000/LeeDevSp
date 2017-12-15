using com.tencent.mars.sample.chat.proto;
using com.tencent.mars.sample.proto;
using ConsoleApp.ConnNetty;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    class Program
    {
        const int CMD_ID_HELLO_VALUE = 1;

        static void Main(string[] args)
        {
            BufferConnection bc = new BufferConnection();
            bc.startConnect();

            ChannelActive(bc);

            //using (MemoryStream ms = new MemoryStream())
            //{
            //    int FIXED_HEADER_SKIP = 4 + 4 + 4 + 4 + 4;
            //    //public int headLength;
            //    //public int clientVersion;
            //    //public int cmdId;
            //    //public int seq;
            //    var req = GetReq();
            //    BinaryWriter writer = new BinaryWriter(ms);
            //    writer.Write(FIXED_HEADER_SKIP);
            //    writer.Write(200);
            //    writer.Write(6);
            //    writer.Write(1);
            //    writer.Write(req.SerializedSize);
            //    writer.Flush();
            //    req.WriteTo(ms);
            //    byte[] buffer = ms.GetBuffer();
            //    bc.send(buffer);
            //}

            bc.Receive();

            Console.Read();
        }

        static void ChannelActive(BufferConnection bc)
        {
            HelloRequest req = new HelloRequest.Builder()
                .SetUser("JackieLee")
                .SetText("Hello proxy")
                .Build();

            NetMsgHeader msgXp = new NetMsgHeader();
            msgXp.cmdId = CMD_ID_HELLO_VALUE;
            msgXp.body = req.ToByteArray();

            byte[] toSendBuf = msgXp.encode();

            bc.Send(toSendBuf);

        }

        static SendMessageRequest GetReq()
        {
            SendMessageRequest req = new SendMessageRequest.Builder()
                .SetAccessToken("test_token")
                .SetFrom("Jackie")
                .SetTo("all")
                .SetText("Hello,everyone.")
                .SetTopic("how to check a test").Build();
            return req;
        }
    }
}


namespace ConsoleApp.ConnNetty
{
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;
    using System;
    using System.IO;
    using System.Threading;
    using System.Text;
    using System.Net;
    using System.Net.Sockets;
    using ICSharpCode.SharpZipLib.Zip;
    using ICSharpCode.SharpZipLib.GZip;
    using LitJson;
    using Diag = System.Diagnostics;
    using ConsoleApp2;

    /**
* 连接对象
* @author duwei
* @version 1.0.0
* build time :2013.11.7
* */
    public class BufferConnection
    {
        public Socket socket = null;
        public const int prefixSize = 4;
        public String ip = "192.168.1.119";
        public int port = 8081;

        private static ManualResetEvent connectDone =
            new ManualResetEvent(false);
        private static ManualResetEvent sendDone =
            new ManualResetEvent(false);
        private static ManualResetEvent receiveDone =
            new ManualResetEvent(false);

        public BufferConnection()
        {

        }
        // State object for receiving data from remote device.
        public class StateObject
        {
            // Client socket.
            public Socket workSocket = null;
            // Size of receive buffer.
            public const int BufferSize = 1024;
            // Receive buffer.
            public byte[] buffer = new byte[BufferSize];
        }
        /**开始建立socket连接*/
        public void startConnect()
        {
            try
            {
                Diag.Debug.WriteLine("starting connection...");
                IPAddress ipd = IPAddress.Parse(ip);
                EndPoint endPoint = new IPEndPoint(ipd, port);

                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.BeginConnect(endPoint, new AsyncCallback(connectCallback), socket);
                connectDone.WaitOne();

                Receive(socket);
                //receiveDone.WaitOne();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        public void connectCallback(IAsyncResult ar)
        {
            try
            {
                Socket backSocket = (Socket)ar.AsyncState;
                backSocket.EndConnect(ar);
                connectDone.Set();
                Diag.Debug.WriteLine("on connected");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        /**发送数据，目前只支持 String 类型数据*/
        public void Send(Socket client, String msg)
        {
            //封装数据
            byte[] byteData = Encoding.UTF8.GetBytes(msg);
            byte[] sendData = new byte[byteData.Length + prefixSize];
            byte[] sizeData = BitConverter.GetBytes(byteData.Length);
            //反转
            Array.Reverse(sizeData);
            //合并
            System.Buffer.BlockCopy(sizeData, 0, sendData, 0, prefixSize);
            System.Buffer.BlockCopy(byteData, 0, sendData, prefixSize, byteData.Length);
            try
            {
                //socket.Send(sendData);
                client.BeginSend(sendData, 0, sendData.Length, 0, new AsyncCallback(SendCallback), client);
                Diag.Debug.WriteLine("data send finished, data size:" + sendData.Length);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public void Send(Socket client, byte[] buffer)
        {
            //封装数据
            byte[] byteData = buffer; //Encoding.UTF8.GetBytes(msg);
            byte[] sendData = new byte[byteData.Length];// + prefixSize];
            byte[] sizeData = BitConverter.GetBytes(byteData.Length);
            //反转
            //Array.Reverse(sizeData);
            ////合并
            //System.Buffer.BlockCopy(sizeData, 0, sendData, 0, prefixSize);
            System.Buffer.BlockCopy(byteData, 0, sendData, 0/*prefixSize*/, byteData.Length);
            try
            {
                //socket.Send(sendData);
                client.BeginSend(sendData, 0, sendData.Length, 0, new AsyncCallback(SendCallback), client);
                Diag.Debug.WriteLine("data send finished, data size:" + sendData.Length);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public void Send(String msg)
        {
            if (socket != null)
            {
                Send(socket, msg);
                sendDone.WaitOne();
            }
        }
        public void Send(byte[] buffer)
        {
            if (socket != null)
            {
                Send(socket, buffer);
                sendDone.WaitOne();
            }
        }

        public void Receive()
        {
            Receive(socket);
        }

        public void SendCallback(IAsyncResult ar)
        {
            try
            {
                Socket socket = (Socket)ar.AsyncState;
                if (ar.IsCompleted)
                {
                    int endPoint = socket.EndSend(ar);
                    Diag.Debug.WriteLine("send data finished endpoint: " + endPoint);
                    sendDone.Set();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        public void Receive(Socket socket)
        {
            try
            {
                StateObject so = new StateObject();
                so.workSocket = socket;
                //第一次读取数据的总长度
                socket.BeginReceive(so.buffer, 0, prefixSize, 0, new AsyncCallback(ReceivedCallback), so);
                //测试用：数据在1024以内的数据，一次性读取出来
                //socket.BeginReceive(so.buffer,0,StateObject.BufferSize,0,new AsyncCallback(simpleCallback),so);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public void SimpleCallback(IAsyncResult ar)
        {
            StateObject so = (StateObject)ar.AsyncState;
            Socket socket = so.workSocket;
            byte[] presixBytes = new byte[prefixSize];
            int presix = 0;
            Buffer.BlockCopy(so.buffer, 0, presixBytes, 0, prefixSize);
            Array.Reverse(presixBytes);
            presix = BitConverter.ToInt32(presixBytes, 0);
            if (presix <= 0)
            {
                return;
            }
            byte[] datas = new byte[presix];
            Buffer.BlockCopy(so.buffer, prefixSize, datas, 0, datas.Length);
            String str = Encoding.UTF8.GetString(datas);
            Diag.Debug.WriteLine("received message :" + str);
        }

        public MemoryStream receiveData = new MemoryStream();
        private bool isPresix = true;
        public int curPrefix = 0;//需要读取的数据总长度

        public void ReceivedCallback(IAsyncResult ar)
        {
            try
            {
                StateObject so = (StateObject)ar.AsyncState;
                Socket client = so.workSocket;
                int readSize = client.EndReceive(ar);//结束读取，返回已读取的缓冲区里的字节数组长度
                                                     //将每次读取的数据，写入内存流里
                receiveData.Write(so.buffer, 0, readSize);
                receiveData.Position = 0;

                NetMsgHeader msgXp = new NetMsgHeader();
                msgXp.decode(receiveData);

            }
            catch
            { }
        }

        public void ReceivedCallback33(IAsyncResult ar)
        {
            try
            {
                StateObject so = (StateObject)ar.AsyncState;
                Socket client = so.workSocket;
                int readSize = client.EndReceive(ar);//结束读取，返回已读取的缓冲区里的字节数组长度
                                                     //将每次读取的数据，写入内存流里
                receiveData.Write(so.buffer, 0, readSize);
                receiveData.Position = 0;
                //读取前置长度，只读取一次
                if ((int)receiveData.Length >= prefixSize && isPresix)
                {
                    byte[] presixBytes = new byte[prefixSize];
                    receiveData.Read(presixBytes, 0, prefixSize);
                    Array.Reverse(presixBytes);
                    curPrefix = BitConverter.ToInt32(presixBytes, 0);
                    isPresix = false;
                }
                if (receiveData.Length - (long)prefixSize < (long)curPrefix)
                {
                    //如果数据没有读取完毕，调整Position到最后，接着读取。
                    receiveData.Position = receiveData.Length;
                }
                else
                {
                    //如果内存流中的实际数字总长度符合要求，则说明数据已经全部读取完毕。
                    //将position位置调整到第4个节点，开始准备读取数据。
                    receiveData.Position = prefixSize;
                    //读取数据
                    byte[] datas = new byte[curPrefix];
                    receiveData.Read(datas, 0, datas.Length);
                    //有压缩的话需要先解压，然后在操作。
                    byte[] finallyBytes = Decompress(datas);
                    String str = Encoding.UTF8.GetString(finallyBytes);
                    Diag.Debug.WriteLine("the finally message is : " + str);
                }
                //重复读取，每次读取1024个字节数据
                client.BeginReceive(so.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceivedCallback), so);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        private byte[] temp = new byte[1024];
        //解压
        public byte[] Decompress(byte[] bytes)
        {
            MemoryStream memory = new MemoryStream();
            ICSharpCode.SharpZipLib.Zip.Compression.Inflater inf = new ICSharpCode.SharpZipLib.Zip.Compression.Inflater();
            inf.SetInput(bytes);
            while (!inf.IsFinished)
            {
                int extracted = inf.Inflate(temp);
                if (extracted > 0)
                {
                    memory.Write(temp, 0, extracted);
                }
                else
                {
                    break;
                }
            }
            return memory.ToArray();
        }
    }
}