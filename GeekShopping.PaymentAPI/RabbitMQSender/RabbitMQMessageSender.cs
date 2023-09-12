using GeekShopping.MessageBus;
using GeekShopping.PaymentAPI.Messages;
using GeekShopping.PaymentAPI.Settings;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace GeekShopping.PaymentAPI.RabbitMQSender
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

        public void SendMessage(BaseMessage message)
        {
            using var channel = Connection.CreateModel();
            channel.ExchangeDeclare(_settings.Exchange, ExchangeType.Direct);
            channel.QueueDeclare(_settings.PaymentEmailUpdateQueue, false, false, false, null);
            channel.QueueDeclare(_settings.PaymentOrderUpdateQueue, false, false, false, null);
            channel.QueueBind(
                _settings.PaymentEmailUpdateQueue, _settings.Exchange, _settings.PaymentEmailRoutingKey);
            channel.QueueBind(
                _settings.PaymentOrderUpdateQueue, _settings.Exchange, _settings.PaymentEmailRoutingKey);

            byte[] body = GetMessageAsByteArray(message);
            channel.BasicPublish(_settings.Exchange, _settings.PaymentEmailRoutingKey, null, body);
            channel.BasicPublish(_settings.Exchange, _settings.PaymentOrderRoutingKey, null, body);
        }

        private static byte[] GetMessageAsByteArray(BaseMessage message)
        {
            var json = JsonSerializer.Serialize(
                (UpdatePaymentResultMessage)message,
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
