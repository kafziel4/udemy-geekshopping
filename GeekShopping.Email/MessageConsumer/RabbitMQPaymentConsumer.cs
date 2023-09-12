using GeekShopping.Email.Messages;
using GeekShopping.Email.Repository;
using GeekShopping.Email.Settings;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace GeekShopping.Email.MessageConsumer
{
    public class RabbitMQPaymentConsumer : BackgroundService
    {
        private readonly IEmailRepository _repository;
        private readonly IModel _channel;
        private readonly RabbitMQSettings _settings;

        public RabbitMQPaymentConsumer(IEmailRepository repository, IOptions<RabbitMQSettings> settings)
        {
            _settings = settings.Value;
            _repository = repository;
            var factory = new ConnectionFactory
            {
                HostName = settings.Value.HostName,
                UserName = settings.Value.UserName,
                Password = settings.Value.Password,
            };
            var connection = factory.CreateConnection();
            _channel = connection.CreateModel();
            _channel.ExchangeDeclare(settings.Value.Exchange, ExchangeType.Direct);
            _channel.QueueDeclare(settings.Value.PaymentEmailUpdateQueue, false, false, false, null);
            _channel.QueueBind(
                settings.Value.PaymentEmailUpdateQueue, settings.Value.Exchange, settings.Value.Exchange);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (channel, evt) =>
            {
                var content = Encoding.UTF8.GetString(evt.Body.ToArray());
                var message = JsonSerializer.Deserialize<UpdatePaymentResultMessage>(content);
                await ProcessLogs(message);
                _channel.BasicAck(evt.DeliveryTag, false);
            };
            _channel.BasicConsume(_settings.PaymentEmailUpdateQueue, false, consumer);
            return Task.CompletedTask;
        }

        private async Task ProcessLogs(UpdatePaymentResultMessage? message)
        {
            if (message is null)
                throw new ArgumentNullException(nameof(message));

            await _repository.LogEmail(message);
        }
    }
}
