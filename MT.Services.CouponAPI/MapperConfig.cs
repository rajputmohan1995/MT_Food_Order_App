using AutoMapper;
using MT.Services.CouponAPI.Models;
using MT.Services.CouponAPI.Models.DTO;

namespace MT.Services.CouponAPI;

public class MapperConfig
{
    public static MapperConfiguration RegisterMaps()
    {
        var mappingConfig = new MapperConfiguration(config =>
        {
            config.CreateMap<Coupon, CouponDTO>();
            config.CreateMap<CouponDTO, Coupon>();
        });
        return mappingConfig;
    }
}
