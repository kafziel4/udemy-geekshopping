using GeekShopping.MessageBus;
using GeekShopping.OrderAPI.Messages;
using GeekShopping.OrderAPI.Settings;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace GeekShopping.OrderAPI.RabbitMQSender
{
    public class RabbitMQMessageSender : IRabbitMQMessageSender
    {
        private readonly RabbitMQSettings _settings;
        private readonly Lazy<IConnection> _connection;

        private IConnection Connection => _connection.Value;

        public RabbitMQMessageSender(IOptions<RabbitMQSettings> settings)
        {
            _settings = settings.Value;
            _connection = new Lazy<IConnection>(() => CreateConnection());
        }

        public void SendMessage(BaseMessage message, string queueName)
        {
            using var channel = Connection.CreateModel();
            channel.QueueDeclare(queue: queueName, false, false, false, arguments: null);
            byte[] body = GetMessageAsByteArray(message);
            channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: null, body: body);
        }

        private static byte[] GetMessageAsByteArray(BaseMessage message)
        {
            var json = JsonSerializer.Serialize(
                (PaymentDto)message,
                new JsonSerializerOptions
                {
                    WriteIndented = true,
                });
            return Encoding.UTF8.GetBytes(json);
        }

        private IConnection CreateConnection()
        {
            var factory = new ConnectionFactory
            {
                HostName = _settings.HostName,
                UserName = _settings.UserName,
                Password = _settings.Password,
            };
            return factory.CreateConnection();
        }
    }
}
