using GeekShopping.CartAPI.Data.DTOs;
using GeekShopping.CartAPI.Messages;
using GeekShopping.CartAPI.RabbitMQSender;
using GeekShopping.CartAPI.Repository;
using GeekShopping.CartAPI.Settings;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;

namespace GeekShopping.CartAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CartsController : ControllerBase
    {
        private readonly ICartRepository _cartRepository;
        private readonly ICouponRepository _couponRepository;
        private readonly IRabbitMQMessageSender _rabbitMQMessageSender;
        private readonly RabbitMQSettings _settings;

        public CartsController(
            ICartRepository repository,
            IRabbitMQMessageSender rabbitMQMessageSender,
            ICouponRepository couponRepository,
            IOptions<RabbitMQSettings> settings)
        {
            _settings = settings.Value;
            _cartRepository = repository ??
                throw new ArgumentNullException(nameof(repository));
            _rabbitMQMessageSender = rabbitMQMessageSender ??
                throw new ArgumentNullException(nameof(rabbitMQMessageSender));
            _couponRepository = couponRepository ??
                throw new ArgumentNullException(nameof(couponRepository));
        }

        [HttpGet]
        public async Task<ActionResult<CartDto>> FindById([Required][FromQuery] string userId)
        {
            var cart = await _cartRepository.FindCartByUserId(userId);
            if (cart is null)
                return NotFound();

            return Ok(cart);
        }

        [HttpPost]
        public async Task<ActionResult<CartDto>> AddCart(CartDto dto)
        {
            var cart = await _cartRepository.SaveOrUpdateCart(dto);
            return Ok(cart);
        }

        [HttpDelete("details/{cartDetailId}")]
        public async Task<ActionResult> RemoveCart(long cartDetailId)
        {
            var isRemoved = await _cartRepository.RemoveFromCart(cartDetailId);
            if (!isRemoved)
                return NotFound();

            return NoContent();
        }

        [HttpPut("coupon")]
        public async Task<ActionResult<CartDto>> ApplyCoupon(
            [Required][FromQuery] string userId, [FromBody] string couponCode)
        {
            var isApplied = await _cartRepository.ApplyCoupon(userId, couponCode);
            if (!isApplied)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("coupon")]
        public async Task<ActionResult<CartDto>> RemoveCoupon([Required][FromQuery] string userId)
        {
            var isRemoved = await _cartRepository.RemoveCoupon(userId);
            if (!isRemoved)
                return NotFound();

            return NoContent();
        }

        [HttpPost("checkout")]
        public async Task<ActionResult<CheckoutHeaderDto>> Checkout(CheckoutHeaderDto dto)
        {
            var cart = await _cartRepository.FindCartByUserId(dto.UserId);
            if (cart is null)
                return NotFound();

            if (!string.IsNullOrEmpty(dto.CouponCode))
            {
                var token = await HttpContext.GetTokenAsync("access_token");
                var coupon = await _couponRepository.GetCoupon(dto.CouponCode, token);
                if (dto.DiscountAmount != coupon.DiscountAmount)
                    return StatusCode(StatusCodes.Status412PreconditionFailed);
            }

            dto.CartDetails = cart.CartDetails;
            dto.DateTime = DateTime.Now;

            _rabbitMQMessageSender.SendMessage(dto, _settings.CheckoutQueue);

            await _cartRepository.ClearCart(dto.UserId);

            return Ok(dto);
        }
    }
}