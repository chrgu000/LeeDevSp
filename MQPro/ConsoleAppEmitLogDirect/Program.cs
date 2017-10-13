using RabbitMQ.Client;
using System;
using System.Linq;
using System.Text;

namespace ConsoleAppEmitLogDirect
{
    /// <summary>
    /// 订阅、发布，路由
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(exchange: "direct_logs", type: "direct");

                    var serverity = (args.Length > 0) ? args[0] : "info";
                    var message = (args.Length > 1) ? string.Join(" ", args.Skip(1).ToArray()) : "Hello Wrold!";
                    var body = Encoding.UTF8.GetBytes(message);
                    channel.BasicPublish(exchange: "direct_logs", routingKey: serverity, basicProperties: null, body:body);
                    Console.WriteLine("[x] Sent '{0}':'{1}'", serverity, message);
                }
                Console.WriteLine("Press [enter] to exit.");
                Console.ReadLine();
            }
        }
    }
}