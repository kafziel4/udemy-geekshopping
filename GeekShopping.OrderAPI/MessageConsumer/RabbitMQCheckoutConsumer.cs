using GeekShopping.OrderAPI.Messages;
using GeekShopping.OrderAPI.Model;
using GeekShopping.OrderAPI.RabbitMQSender;
using GeekShopping.OrderAPI.Repository;
using GeekShopping.OrderAPI.Settings;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace GeekShopping.OrderAPI.MessageConsumer
{
    public class RabbitMQCheckoutConsumer : BackgroundService
    {
        private readonly IOrderRepository _repository;
        private readonly IModel _channel;
        private readonly IRabbitMQMessageSender _rabbitMQMessageSender;
        private readonly RabbitMQSettings _settings;

        public RabbitMQCheckoutConsumer(
            IOrderRepository repository,
            IRabbitMQMessageSender rabbitMQMessageSender,
            IOptions<RabbitMQSettings> settings)
        {
            _settings = settings.Value;
            _repository = repository;
            _rabbitMQMessageSender = rabbitMQMessageSender;
            var factory = new ConnectionFactory
            {
                HostName = settings.Value.HostName,
                UserName = settings.Value.UserName,
                Password = settings.Value.Password,
            };
            var connection = factory.CreateConnection();
            _channel = connection.CreateModel();
            _channel.QueueDeclare(queue: settings.Value.CheckoutQueue, false, false, false, arguments: null);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (channel, evt) =>
            {
                var content = Encoding.UTF8.GetString(evt.Body.ToArray());
                var dto = JsonSerializer.Deserialize<CheckoutHeaderDto>(content);
                await ProcessOrder(dto);
                _channel.BasicAck(evt.DeliveryTag, false);
            };
            _channel.BasicConsume(_settings.CheckoutQueue, false, consumer);
            return Task.CompletedTask;
        }

        private async Task ProcessOrder(CheckoutHeaderDto? dto)
        {
            if (dto is null)
                throw new ArgumentNullException(nameof(dto));

            var order = new OrderHeader
            {
                UserId = dto.UserId,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                CardNumber = dto.CardNumber,
                CouponCode = dto.CouponCode,
                CVV = dto.CVV,
                PurchaseAmount = dto.PurchaseAmount,
                DiscountAmount = dto.DiscountAmount,
                Email = dto.Email,
                ExpiryMonthYear = dto.ExpiryMonthYear,
                OrderTime = DateTime.Now,
                PaymentStatus = false,
                Phone = dto.Phone,
                DateTime = dto.DateTime,
            };

            foreach (var details in dto.CartDetails)
            {
                if (details.Product is null)
                    throw new ArgumentException($"{nameof(details.Product)} is null");

                var detail = new OrderDetail
                {
                    ProductId = details.ProductId,
                    ProductName = details.Product.Name,
                    Price = details.Product.Price,
                    Count = details.Count
                };
                order.OrderTotalItems += details.Count;
                order.OrderDetails.Add(detail);
            }

            await _repository.AddOrder(order);

            var payment = new PaymentDto
            {
                Name = $"{order.FirstName} {order.LastName}",
                CardNumber = order.CardNumber,
                CVV = order.CVV,
                ExpiryMonthYear = order.ExpiryMonthYear,
                OrderId = order.Id,
                PurchaseAmount = order.PurchaseAmount,
                Email = order.Email,
            };

            _rabbitMQMessageSender.SendMessage(payment, _settings.OrderPaymentProcessQueue);
        }
    }
}
