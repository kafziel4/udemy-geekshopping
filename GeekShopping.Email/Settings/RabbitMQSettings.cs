namespace GeekShopping.Email.Settings
{
    public class RabbitMQSettings
    {
        public string HostName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Exchange { get; set; } = string.Empty;
        public string PaymentEmailUpdateQueue { get; set; } = string.Empty;
        public string PaymentEmailRoutingKey { get; set; } = string.Empty;
    }
}
