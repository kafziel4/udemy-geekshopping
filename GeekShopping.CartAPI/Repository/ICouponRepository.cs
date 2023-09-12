using GeekShopping.CartAPI.Data.DTOs;

namespace GeekShopping.CartAPI.Repository
{
    public interface ICouponRepository
    {
        Task<CouponDto> GetCoupon(string couponCode, string? token);
    }
}
