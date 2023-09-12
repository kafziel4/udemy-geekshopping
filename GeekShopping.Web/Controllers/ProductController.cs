using GeekShopping.Web.Models;
using GeekShopping.Web.Services.IServices;
using GeekShopping.Web.Utils;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GeekShopping.Web.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
        }

        [AllowAnonymous]
        public async Task<IActionResult> ProductIndex()
        {
            var products = await _productService.FindAllProducts(null);
            return View(products);
        }

        public IActionResult ProductCreate()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ProductCreate(ProductViewModel model)
        {
            if (ModelState.IsValid)
            {
                var token = await HttpContext.GetTokenAsync("access_token");
                var response = await _productService.CreateProduct(model, token);
                if (response is not null)
                    return RedirectToAction(nameof(ProductIndex));
            }

            return View(model);
        }

        public async Task<IActionResult> ProductUpdate(long id)
        {
            var token = await HttpContext.GetTokenAsync("access_token");
            var model = await _productService.FindProductById(id, token);
            if (model is null)
                return NotFound();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ProductUpdate(ProductViewModel model)
        {
            if (ModelState.IsValid)
            {
                var token = await HttpContext.GetTokenAsync("access_token");
                var response = await _productService.UpdateProduct(model, token);
                if (response is not null)
                    return RedirectToAction(nameof(ProductIndex));
            }

            return View(model);
        }

        public async Task<IActionResult> ProductDelete(long id)
        {
            var token = await HttpContext.GetTokenAsync("access_token");
            var model = await _productService.FindProductById(id, token);
            if (model is null)
                return NotFound();

            return View(model);
        }

        [Authorize(Roles = Role.Admin)]
        [HttpPost]
        public async Task<IActionResult> ProductDelete(ProductViewModel model)
        {
            var token = await HttpContext.GetTokenAsync("access_token");
            var isDeleted = await _productService.DeleteProductById(model.Id, token);
            if (isDeleted)
                return RedirectToAction(nameof(ProductIndex));


            return View(model);
        }
    }
}
