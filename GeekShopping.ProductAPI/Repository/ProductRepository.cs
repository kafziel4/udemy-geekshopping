using AutoMapper;
using GeekShopping.ProductAPI.Data.DTOs;
using GeekShopping.ProductAPI.Model;
using GeekShopping.ProductAPI.Model.Context;
using Microsoft.EntityFrameworkCore;

namespace GeekShopping.ProductAPI.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly MySqlContext _context;
        private readonly IMapper _mapper;

        public ProductRepository(MySqlContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductDto>> FindAll()
        {
            var products = await _context.Products.ToListAsync();
            return _mapper.Map<List<ProductDto>>(products);
        }
        public async Task<ProductDto?> FindById(long id)
        {
            var product = await _context.Products.FindAsync(id);
            return _mapper.Map<ProductDto>(product);
        }


        public async Task<ProductDto> Create(ProductDto dto)
        {
            var product = _mapper.Map<Product>(dto);
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return _mapper.Map<ProductDto>(product);
        }

        public async Task<ProductDto?> Update(ProductDto dto)
        {
            var product = await _context.Products.FindAsync(dto.Id);
            if (product is null)
                return null;

            _mapper.Map(dto, product);
            await _context.SaveChangesAsync();

            return _mapper.Map<ProductDto>(product);
        }

        public async Task<bool> Delete(long id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product is null)
                return false;

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
