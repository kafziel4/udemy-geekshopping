using GeekShopping.Web.Exceptions;
using GeekShopping.Web.Models;
using GeekShopping.Web.Services.IServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GeekShopping.Web.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICartService _cartService;
        private readonly ICouponService _couponService;

        public CartController(
            IProductService productService,
            ICartService cartService,
            ICouponService couponService)
        {
            _productService = productService;
            _cartService = cartService;
            _couponService = couponService;
        }

        public async Task<IActionResult> CartIndex()
        {
            return View(await FindUserCart());
        }

        public async Task<IActionResult> Remove(int id)
        {
            var token = await HttpContext.GetTokenAsync("access_token");

            var isRemoved = await _cartService.RemoveFromCart(id, token);
            if (isRemoved)
                return RedirectToAction(nameof(CartIndex));

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ApplyCoupon(string couponCode)
        {
            var token = await HttpContext.GetTokenAsync("access_token");
            var userId = User.Claims.First(u => u.Type == "sub").Value;

            var isApplied = await _cartService.ApplyCoupon(userId, couponCode, token);
            if (isApplied)
                return RedirectToAction(nameof(CartIndex));

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RemoveCoupon()
        {
            var token = await HttpContext.GetTokenAsync("access_token");
            var userId = User.Claims.First(u => u.Type == "sub").Value;

            var isRemoved = await _cartService.RemoveCoupon(userId, token);
            if (isRemoved)
                return RedirectToAction(nameof(CartIndex));

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            return View(await FindUserCart());
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(CartViewModel model)
        {
            try
            {
                var token = await HttpContext.GetTokenAsync("access_token");

                var response = await _cartService.Checkout(model.CartHeader, token);
                if (response is not null)
                    return RedirectToAction(nameof(Confirmation));
            }
            catch (PreconditionException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Checkout));
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Confirmation()
        {
            return View();
        }

        private async Task<CartViewModel?> FindUserCart()
        {
            var token = await HttpContext.GetTokenAsync("access_token");
            var userId = User.Claims.First(u => u.Type == "sub").Value;

            var response = await _cartService.FindCartByUserId(userId, token);
            if (response?.CartHeader is not null)
            {
                if (!string.IsNullOrEmpty(response.CartHeader.CouponCode))
                {
                    var coupon = await _couponService
                        .GetCoupon(response.CartHeader.CouponCode, token);

                    response.CartHeader.DiscountAmount = coupon.DiscountAmount;
                }

                foreach (var detail in response.CartDetails.Where(d => d.Product is not null))
                {
                    response.CartHeader.PurchaseAmount += detail.Product!.Price * detail.Count;
                }

                response.CartHeader.PurchaseAmount -= response.CartHeader.DiscountAmount;
            }

            return response;
        }
    }
}
