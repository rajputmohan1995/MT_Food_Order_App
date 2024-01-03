using AutoMapper;
using MT.Services.ShoppingCartAPI.Models;
using MT.Services.ShoppingCartAPI.Models.DTO;

namespace MT.Services.ShoppingCartAPI.Extensions;

public class MapperConfig
{
    public static MapperConfiguration RegisterMaps()
    {
        var mappingConfig = new MapperConfiguration(config =>
        {
            config.CreateMap<CartHeader, CartHeaderDTO>().ReverseMap();
            config.CreateMap<CartDetail, CartDetailDTO>().ReverseMap();
        });
        return mappingConfig;
    }
}