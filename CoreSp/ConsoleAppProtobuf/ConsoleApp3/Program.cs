using com.tencent.mars.sample.proto;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Handlers.Logging;
using DotNetty.Handlers.Tls;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp3
{
    class Program
    {
        static void Main(string[] args)
        {
            Task.FromResult(RunClientAsync());

            Console.Read();
        }

        static async Task RunClientAsync()
        {
            //ExampleHelper.SetConsoleLogger();

            var group = new MultithreadEventLoopGroup();

            X509Certificate2 cert = null;
            string targetHost = null;
            //if (ClientSettings.IsSsl)
            //{
            //    cert = new X509Certificate2(Path.Combine(ExampleHelper.ProcessDirectory, "dotnetty.com.pfx"), "password");
            //    targetHost = cert.GetNameInfo(X509NameType.DnsName, false);
            //}
            try
            {
                var bootstrap = new Bootstrap();
                bootstrap
                    .Group(group)
                    .Channel<TcpSocketChannel>()
                    .Option(ChannelOption.TcpNodelay, true)
                    .Handler(new ActionChannelInitializer<ISocketChannel>(channel =>
                    {
                        IChannelPipeline pipeline = channel.Pipeline;

                        if (cert != null)
                        {
                            pipeline.AddLast("tls", new TlsHandler(stream => new SslStream(stream, true, (sender, certificate, chain, errors) => true), new ClientTlsSettings(targetHost)));
                        }
                        //pipeline.AddLast(new LoggingHandler());
                        //pipeline.AddLast("framing-enc", new LengthFieldPrepender(2));
                        //pipeline.AddLast("framing-dec", new LengthFieldBasedFrameDecoder(ushort.MaxValue, 0, 2, 0, 2));

                        pipeline.AddLast("echo", new EchoClientHandler());
                    }));

                IChannel clientChannel = await bootstrap.ConnectAsync(new IPEndPoint(new IPAddress(new byte[] { 192, 168, 1, 119 }), 8081));

                Console.ReadLine();

                await clientChannel.CloseAsync();
            }
            finally
            {
                await group.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1));
            }
        }
    }

    // 代码和服务端也相差不多，并且继承了同样的基类。
    public class EchoClientHandler : ChannelHandlerAdapter
    {
        const int CMD_ID_HELLO_VALUE = 1;

        readonly IByteBuffer initialMessage;
        NetMsgHeader msgXp = new NetMsgHeader();

        public EchoClientHandler()
        {
            this.initialMessage = Unpooled.Buffer(1024); // ClientSettings.Size);
            byte[] messageBytes = Encoding.UTF8.GetBytes("Hello world");
            this.initialMessage.WriteBytes(messageBytes);
        }

        //重写基类方法，当链接上服务器后，马上发送Hello World消息到服务端
        public override void ChannelActive(IChannelHandlerContext context)
        {
            HelloRequest req = new HelloRequest.Builder()
                .SetUser("JackieLee")
                .SetText("Hello proxy")
                .Build();

            msgXp.cmdId = CMD_ID_HELLO_VALUE;
            msgXp.body = req.ToByteArray();

            byte[] toSendBuf = msgXp.encode();
            IByteBuffer encoded = context.Allocator.Buffer(toSendBuf.Length);
            encoded.WriteBytes(toSendBuf);
            context.WriteAndFlushAsync(encoded);
        }

        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            var byteBuffer = message as IByteBuffer;
            msgXp.decode((Stream)byteBuffer);

            HelloResponse resp = HelloResponse.ParseFrom(msgXp.body);
            "resp decoded, resp.retcode={0}, resp.err={1}".Log(resp.Retcode, resp.Errmsg);

            context.WriteAsync(message);

            return;

            if (byteBuffer != null)
            {
                Console.WriteLine("Received from server: " + byteBuffer.ToString(Encoding.UTF8));
            }
            context.WriteAsync(message);
        }

        public override void ChannelReadComplete(IChannelHandlerContext context) => context.Flush();

        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            Console.WriteLine("Exception: " + exception);
            context.CloseAsync();
        }
    }
}
