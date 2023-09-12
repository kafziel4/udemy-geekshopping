using AutoMapper;
using GeekShopping.ProductAPI.Data.DTOs;
using GeekShopping.ProductAPI.Model;

namespace GeekShopping.ProductAPI.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ProductDto, Product>().ReverseMap();
        }

    }
}
