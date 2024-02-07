using AutoMapper;
using MT.Services.OrderAPI.Models;
using MT.Services.OrderAPI.Models.DTO;

namespace MT.Services.OrderAPI.Extensions;

public class MapperConfig
{
    public static MapperConfiguration RegisterMaps()
    {
        var mappingConfig = new MapperConfiguration(config =>
        {
            config.CreateMap<OrderHeaderDTO, CartHeaderDTO>()
                .ForMember(dest => dest.CartTotal, u => u.MapFrom(src => src.OrderTotal)).ReverseMap();

            config.CreateMap<CartDetailDTO, OrderDetailDTO>()
                .ForMember(dest => dest.ProductName, u => u.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.ProductPrice, u => u.MapFrom(src => src.Product.Price));

            config.CreateMap<OrderDetailDTO, CartDetailDTO>();

            config.CreateMap<OrderHeader, OrderHeaderDTO>().ReverseMap();
            config.CreateMap<OrderDetail, OrderDetailDTO>().ReverseMap();
        });
        return mappingConfig;
    }
}