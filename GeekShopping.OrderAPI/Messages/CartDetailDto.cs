namespace GeekShopping.OrderAPI.Data.DTOs
{
    public class CartDetailDto
    {
        public long Id { get; set; }
        public long CartHeaderId { get; set; }
        public long ProductId { get; set; }
        public ProductDto? Product { get; set; }
        public int Count { get; set; }
    }
}
