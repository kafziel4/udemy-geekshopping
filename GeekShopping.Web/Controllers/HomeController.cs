using GeekShopping.Web.Models;
using GeekShopping.Web.Services.IServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace GeekShopping.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICartService _cartService;

        public HomeController(IProductService productService, ICartService cartService)
        {
            _productService = productService;
            _cartService = cartService;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _productService.FindAllProducts(null);
            return View(products);
        }

        [Authorize]
        public async Task<IActionResult> Details(long id)
        {
            var token = await HttpContext.GetTokenAsync("access_token");
            var model = await _productService.FindProductById(id, token);
            return View(model);
        }

        [ActionName("Details")]
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> DetailsPost(ProductViewModel model)
        {
            var token = await HttpContext.GetTokenAsync("access_token");

            var cartHeader = new CartHeaderViewModel
            {
                UserId = User.Claims.First(u => u.Type == "sub").Value
            };

            var cartDetails = new List<CartDetailViewModel>
            {
                new CartDetailViewModel
                {
                    Count = model.Count,
                    ProductId = model.Id,
                    Product = await _productService.FindProductById(model.Id, token)
                }
            };

            var cart = new CartViewModel
            {
                CartHeader = cartHeader,
                CartDetails = cartDetails
            };

            var response = await _cartService.AddOrUpdateCart(cart, token);
            if (response is not null)
                return RedirectToAction(nameof(Index));

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Authorize]
        public IActionResult Login()
        {
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Logout()
        {
            return SignOut("Cookies", "oidc");
        }
    }
}