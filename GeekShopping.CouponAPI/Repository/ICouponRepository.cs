using GeekShopping.CouponAPI.Data.DTOs;

namespace GeekShopping.CouponAPI.Repository
{
    public interface ICouponRepository
    {
        Task<CouponDto> GetCouponByCouponCode(string couponCode);
    }
}
