using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace ConsoleAppReceiveLogDirect
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
                    var queueName = channel.QueueDeclare().QueueName;
                    if(args.Length < 1)
                    {
                        Console.Error.WriteLine("Usage: {0} [info] [warning] [error]", Environment.GetCommandLineArgs()[0]);
                        Console.ReadLine();
                        Environment.Exit(1);
                        return;
                    }
                    foreach(var serverity in args)
                    {
                        channel.QueueBind(queue: queueName, exchange: "direct_logs", routingKey: serverity);
                    }
                    Console.WriteLine("[*] Warning for message.");

                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                      {
                          var body = ea.Body;
                          var message = Encoding.UTF8.GetString(body);
                          var routingKey = ea.RoutingKey;
                          Console.WriteLine("[*] Received '{0}':'{1}'", routingKey, message);
                      };
                    channel.BasicConsume(queue: queueName, noAck: true, consumer: consumer);

                    Console.WriteLine("Press [enter] to exit.");
                    Console.ReadLine();
                }
            }
        }
    }
}