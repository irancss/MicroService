using System.Text;
using RabbitMQ.Client;

namespace Notification.messaging.rabbitmq
{
    class Program
    {
        static void Main(string[] args)
        {
            // Producer
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection()) // Fix: Ensure RabbitMQ.Client is referenced in the project
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: "orders", type: "direct");
                channel.QueueDeclare(queue: "inventory_queue", durable: false, exclusive: false, autoDelete: false, arguments: null);
                channel.QueueBind(queue: "inventory_queue", exchange: "orders", routingKey: "order.placed");

                string message = "New order placed!";
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "orders", routingKey: "order.placed", basicProperties: null, body: body);
                Console.WriteLine(" [x] Sent {0}", message);
            }

            // Consumer
            using (var connection = factory.CreateConnection()) // Fix: Ensure RabbitMQ.Client is referenced in the project
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "inventory_queue", durable: false, exclusive: false, autoDelete: false, arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine(" [x] Received {0}", message);
                };
                channel.BasicConsume(queue: "inventory_queue", autoAck: true, consumer: consumer);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }
    }
}
