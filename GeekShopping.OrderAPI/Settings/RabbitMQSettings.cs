namespace GeekShopping.OrderAPI.Settings
{
    public class RabbitMQSettings
    {
        public string HostName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Exchange { get; set; } = string.Empty;
        public string PaymentOrderUpdateQueue { get; set; } = string.Empty;
        public string PaymentOrderRoutingKey { get; set; } = string.Empty;
        public string CheckoutQueue { get; set; } = string.Empty;
        public string OrderPaymentProcessQueue { get; set; } = string.Empty;
    }
}
