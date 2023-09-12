using AutoMapper;
using GeekShopping.CartAPI.Data.DTOs;
using GeekShopping.CartAPI.Model;
using GeekShopping.CartAPI.Model.Context;
using Microsoft.EntityFrameworkCore;

namespace GeekShopping.CartAPI.Repository
{
    public class CartRepository : ICartRepository
    {
        private readonly MySqlContext _context;
        private readonly IMapper _mapper;

        public CartRepository(MySqlContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<CartDto?> FindCartByUserId(string userId)
        {
            var cartHeader = await _context.CartHeaders
                .FirstOrDefaultAsync(c => c.UserId == userId);
            if (cartHeader is null)
                return null;

            var cartDetails = await _context.CartDetails
                .Where(c => c.CartHeaderId == cartHeader.Id)
                .Include(c => c.Product)
                .ToListAsync();

            var cart = new Cart
            {
                CartHeader = cartHeader,
                CartDetails = cartDetails
            };

            return _mapper.Map<CartDto>(cart);
        }

        public async Task<CartDto> SaveOrUpdateCart(CartDto dto)
        {
            var cart = _mapper.Map<Cart>(dto);

            foreach (var detail in cart.CartDetails)
            {
                var product = await _context.Products
                    .FirstOrDefaultAsync(p => p.Id == detail.ProductId);
                if (product is null && detail.Product is not null)
                    _context.Products.Add(detail.Product);

                detail.Product = null;
            }

            var cartHeader = await _context.CartHeaders
                .FirstOrDefaultAsync(c => c.UserId == cart.CartHeader.UserId);
            if (cartHeader is null)
            {
                _context.CartHeaders.Add(cart.CartHeader);
                foreach (var detail in cart.CartDetails)
                {
                    detail.CartHeader = cart.CartHeader;
                    _context.CartDetails.Add(detail);
                }
            }
            else
            {
                foreach (var detail in cart.CartDetails)
                {
                    var cartDetail = await _context.CartDetails
                        .FirstOrDefaultAsync(c => c.CartHeaderId == cartHeader.Id &&
                            c.ProductId == detail.ProductId);
                    if (cartDetail is null)
                    {
                        detail.CartHeaderId = cartHeader.Id;
                        _context.CartDetails.Add(detail);
                    }
                    else
                    {
                        cartDetail.Count += detail.Count;
                    }
                }
            }

            await _context.SaveChangesAsync();
            return _mapper.Map<CartDto>(cart);
        }

        public async Task<bool> RemoveFromCart(long cartDetailId)
        {
            var cartDetail = await _context.CartDetails
                .FindAsync(cartDetailId);
            if (cartDetail is null)
                return false;

            _context.CartDetails.Remove(cartDetail);

            var total = _context.CartDetails
                .Where(c => c.CartHeaderId == cartDetail.CartHeaderId).Count();

            if (total == 1)
            {
                var cartHeaderToRemove = await _context.CartHeaders
                    .FirstAsync(c => c.Id == cartDetail.CartHeaderId);
                _context.CartHeaders.Remove(cartHeaderToRemove);
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ApplyCoupon(string userId, string couponCode)
        {
            var cartHeader = await _context.CartHeaders
                .FirstOrDefaultAsync(c => c.UserId == userId);
            if (cartHeader is null)
                return false;

            cartHeader.CouponCode = couponCode;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RemoveCoupon(string userId)
        {
            var cartHeader = await _context.CartHeaders
                .FirstOrDefaultAsync(c => c.UserId == userId);
            if (cartHeader is null)
                return false;

            cartHeader.CouponCode = null;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ClearCart(string userId)
        {
            var cartHeader = await _context.CartHeaders
                .FirstOrDefaultAsync(c => c.UserId == userId);
            if (cartHeader is null)
                return false;

            _context.CartHeaders.Remove(cartHeader);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
