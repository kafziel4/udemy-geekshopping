namespace GeekShopping.PaymentAPI.Settings
{
    public class RabbitMQSettings
    {
        public string HostName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Exchange { get; set; } = string.Empty;
        public string PaymentEmailUpdateQueue { get; set; } = string.Empty;
        public string PaymentEmailRoutingKey { get; set; } = string.Empty;
        public string PaymentOrderUpdateQueue { get; set; } = string.Empty;
        public string PaymentOrderRoutingKey { get; set; } = string.Empty;
        public string OrderPaymentProcessQueue { get; set; } = string.Empty;
    }
}
