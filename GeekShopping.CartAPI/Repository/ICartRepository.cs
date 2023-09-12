using GeekShopping.CartAPI.Data.DTOs;

namespace GeekShopping.CartAPI.Repository
{
    public interface ICartRepository
    {
        Task<CartDto?> FindCartByUserId(string userId);
        Task<CartDto> SaveOrUpdateCart(CartDto dto);
        Task<bool> RemoveFromCart(long cartDetailId);
        Task<bool> ApplyCoupon(string userId, string couponCode);
        Task<bool> RemoveCoupon(string userId);
        Task<bool> ClearCart(string userId);
    }
}
