namespace GeekShopping.CartAPI.Data.DTOs
{
    public class CartDto
    {
        public CartHeaderDto CartHeader { get; set; } = null!;
        public IEnumerable<CartDetailDto> CartDetails { get; set; } = new List<CartDetailDto>();
    }
}
