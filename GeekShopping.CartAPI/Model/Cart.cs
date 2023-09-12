namespace GeekShopping.CartAPI.Model
{
    public class Cart
    {
        public CartHeader CartHeader { get; set; } = null!;
        public IEnumerable<CartDetail> CartDetails { get; set; } = new List<CartDetail>();
    }
}
