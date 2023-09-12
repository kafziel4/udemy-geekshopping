namespace GeekShopping.CartAPI.Settings
{
    public class RabbitMQSettings
    {
        public string HostName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string CheckoutQueue { get; set; } = string.Empty;
    }
}
