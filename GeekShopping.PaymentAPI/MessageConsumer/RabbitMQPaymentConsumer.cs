using GeekShopping.PaymentAPI.Messages;
using GeekShopping.PaymentAPI.RabbitMQSender;
using GeekShopping.PaymentAPI.Settings;
using GeekShopping.PaymentProcessor;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace GeekShopping.PaymentAPI.MessageConsumer
{
    public class RabbitMQPaymentConsumer : BackgroundService
    {
        private readonly IModel _channel;
        private readonly IProcessPayment _processPayment;
        private readonly IRabbitMQMessageSender _rabbitMQMessageSender;
        private readonly RabbitMQSettings _settings;

        public RabbitMQPaymentConsumer(
            IProcessPayment processPayment,
            IRabbitMQMessageSender rabbitMQMessageSender,
            IOptions<RabbitMQSettings> settings)
        {
            _settings = settings.Value;
            _processPayment = processPayment;
            _rabbitMQMessageSender = rabbitMQMessageSender;
            var factory = new ConnectionFactory
            {
                HostName = settings.Value.HostName,
                UserName = settings.Value.UserName,
                Password = settings.Value.Password,
            };
            var connection = factory.CreateConnection();
            _channel = connection.CreateModel();
            _channel.QueueDeclare(
                queue: settings.Value.OrderPaymentProcessQueue, false, false, false, arguments: null);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (channel, evt) =>
            {
                var content = Encoding.UTF8.GetString(evt.Body.ToArray());
                var dto = JsonSerializer.Deserialize<PaymentMessage>(content);
                ProcessPayment(dto);
                _channel.BasicAck(evt.DeliveryTag, false);
            };
            _channel.BasicConsume(_settings.OrderPaymentProcessQueue, false, consumer);
            return Task.CompletedTask;
        }

        private void ProcessPayment(PaymentMessage? dto)
        {
            if (dto is null)
                throw new ArgumentNullException(nameof(dto));

            var result = _processPayment.PaymentProcessor();
            var paymentResult = new UpdatePaymentResultMessage
            {
                Status = result,
                OrderId = dto.OrderId,
                Email = dto.Email,
            };
            _rabbitMQMessageSender.SendMessage(paymentResult);
        }
    }
}
