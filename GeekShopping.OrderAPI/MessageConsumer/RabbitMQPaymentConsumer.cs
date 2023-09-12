using GeekShopping.OrderAPI.Messages;
using GeekShopping.OrderAPI.Repository;
using GeekShopping.OrderAPI.Settings;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace GeekShopping.OrderAPI.MessageConsumer
{
    public class RabbitMQPaymentConsumer : BackgroundService
    {
        private readonly IOrderRepository _repository;
        private readonly IModel _channel;
        private readonly RabbitMQSettings _settings;

        public RabbitMQPaymentConsumer(IOrderRepository repository, IOptions<RabbitMQSettings> settings)
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
            _channel.QueueDeclare(settings.Value.PaymentOrderUpdateQueue, false, false, false, null);
            _channel.QueueBind(
                settings.Value.PaymentOrderUpdateQueue,
                settings.Value.Exchange,
                settings.Value.PaymentOrderRoutingKey);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (channel, evt) =>
            {
                var content = Encoding.UTF8.GetString(evt.Body.ToArray());
                var dto = JsonSerializer.Deserialize<UpdatePaymentResultDto>(content);
                await UpdatePaymentStatus(dto);
                _channel.BasicAck(evt.DeliveryTag, false);
            };
            _channel.BasicConsume(_settings.PaymentOrderUpdateQueue, false, consumer);
            return Task.CompletedTask;
        }

        private async Task UpdatePaymentStatus(UpdatePaymentResultDto? dto)
        {
            if (dto is null)
                throw new ArgumentNullException(nameof(dto));

            await _repository.UpdateOrderPaymentStatus(dto.OrderId, dto.Status);
        }
    }
}
