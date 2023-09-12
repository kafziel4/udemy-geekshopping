namespace GeekShopping.Web.Models
{
    public class CartViewModel
    {
        public CartHeaderViewModel CartHeader { get; set; } = null!;
        public IEnumerable<CartDetailViewModel> CartDetails { get; set; } = new List<CartDetailViewModel>();
    }
}
