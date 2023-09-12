namespace GeekShopping.CartAPI.Data.DTOs
{
    public class CouponDto
    {
        public long Id { get; set; }
        public string CouponCode { get; set; } = string.Empty;
        public decimal DiscountAmount { get; set; }
    }
}
