using GeekShopping.Web.Models;

namespace GeekShopping.Web.Services.IServices
{
    public interface ICartService
    {
        Task<CartViewModel?> FindCartByUserId(string userId, string? token);
        Task<CartViewModel?> AddOrUpdateCart(CartViewModel model, string? token);
        Task<bool> RemoveFromCart(long cartDetailsId, string? token);
        Task<bool> ApplyCoupon(string userId, string couponCode, string? token);
        Task<bool> RemoveCoupon(string userId, string? token);
        Task<CartHeaderViewModel?> Checkout(CartHeaderViewModel cartHeader, string? token);
        Task<bool> ClearCart(string userId, string? token);
    }
}
