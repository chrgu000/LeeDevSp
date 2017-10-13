using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;

namespace ConsoleAppWorker
{
    /// <summary>
    /// 可持久化队列
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
                    channel.QueueDeclare(queue: "task_queue", durable: true, exclusive: false, autoDelete: false, arguments: null);
                    channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false); // 只有处理完一条消息并回应后，才接收下一条
                    Console.WriteLine("[*] Waiting for messages:");
                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                      {
                          var body = ea.Body;
                          var message = Encoding.UTF8.GetString(body);
                          Console.WriteLine("[*] Received {0}", message);

                          int dots = message.Split('.').Length - 1;
                          Thread.Sleep(dots * 1000); // 每个.停1秒
                          Console.WriteLine("[*] Done");

                          channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false); // 单条
                      };
                    channel.BasicConsume(queue: "task_queue", noAck: false, consumer: consumer);

                    Console.WriteLine("Press [enter] to exit.");
                    Console.ReadLine();
                }
            }
        }
    }
}