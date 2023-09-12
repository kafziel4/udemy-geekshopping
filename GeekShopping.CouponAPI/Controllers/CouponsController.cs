using GeekShopping.CouponAPI.Data.DTOs;
using GeekShopping.CouponAPI.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GeekShopping.CouponAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CouponsController : ControllerBase
    {
        private readonly ICouponRepository _repository;

        public CouponsController(ICouponRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("{couponCode}")]
        public async Task<ActionResult<CouponDto>> FindByCode(string couponCode)
        {
            var coupon = await _repository.GetCouponByCouponCode(couponCode);
            if (coupon is null)
                return NotFound();

            return Ok(coupon);
        }
    }
}