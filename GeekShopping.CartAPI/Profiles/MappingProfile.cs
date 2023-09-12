using AutoMapper;
using GeekShopping.CartAPI.Data.DTOs;
using GeekShopping.CartAPI.Model;

namespace GeekShopping.CartAPI.Config
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ProductDto, Product>().ReverseMap();
            CreateMap<CartHeaderDto, CartHeader>().ReverseMap();
            CreateMap<CartDetailDto, CartDetail>().ReverseMap();
            CreateMap<CartDto, Cart>().ReverseMap();
        }
    }
}
