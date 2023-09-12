using GeekShopping.ProductAPI.Data.DTOs;
using GeekShopping.ProductAPI.Repository;
using GeekShopping.ProductAPI.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GeekShopping.ProductAPI.Controllers
{
    [Authorize]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _repository;

        public ProductsController(IProductRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> FindAll()
        {
            var products = await _repository.FindAll();
            return Ok(products);
        }

        [HttpGet("{id}", Name = "GetProduct")]
        public async Task<ActionResult<ProductDto>> FindById(long id)
        {
            var product = await _repository.FindById(id);
            if (product is null)
                return NotFound();

            return Ok(product);
        }

        [HttpPost]
        public async Task<ActionResult<ProductDto>> Create(ProductDto dto)
        {
            var product = await _repository.Create(dto);
            return CreatedAtRoute("GetProduct", new { id = product.Id }, product);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ProductDto>> Update(long id, ProductDto dto)
        {
            if (id != dto.Id)
                return BadRequest();

            var product = await _repository.Update(dto);
            if (product is null)
                return NotFound();

            return Ok(product);
        }

        [Authorize(Roles = Role.Admin)]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(long id)
        {
            var isDeleted = await _repository.Delete(id);
            if (!isDeleted)
                return NotFound();

            return NoContent();
        }
    }
}
