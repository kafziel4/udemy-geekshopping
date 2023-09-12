using AutoMapper;
using GeekShopping.CouponAPI.Data.DTOs;
using GeekShopping.CouponAPI.Model;

namespace GeekShopping.CouponAPI.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CouponDto, Coupon>().ReverseMap();
        }
    }
}
