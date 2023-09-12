namespace GeekShopping.CartAPI.Data.DTOs
{
    public class CartHeaderDto
    {
        public long Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string? CouponCode { get; set; }
    }
}
